/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2014 Andrew B. Johnson
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using Functional.Maybe;
using Sprache;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FansubFileNameParser
{
    /// <summary>
    /// Extra parsers that perform specialized parsing and safe type deduction
    /// </summary>
    internal static class ExtraParsers
    {
        /// <summary>
        /// Parses a number from a string and safely casts it to an integer
        /// </summary>
        public static readonly Parser<int> Int =
            from intAsString in Parse.Number
            let parseResult = TryParseInt(intAsString)
            where parseResult.IsSomething()
            select parseResult.Value;

        /// <summary>
        /// Implodes this <see cref="Parser{T}"/>'s results from a nested <see cref="IEnumerable{T}"/> to a single
        /// IEnumerable{T}.
        /// </summary>
        /// <typeparam name="T">The element type</typeparam>
        /// <param name="this">The parser</param>
        /// <returns>A new parser that implodes the return value of the parser</returns>
        public static Parser<IEnumerable<T>> Implode<T>(this Parser<IEnumerable<IEnumerable<T>>> @this)
        {
            return input =>
            {
                var result = @this.Invoke(input);
                if (result.WasSuccessful == false)
                {
                    return Result.Failure<IEnumerable<T>>(
                        result.Remainder,
                        string.Format("Failure to implode. Dependent parser failed. {0}", result.Message), 
                        result.Expectations
                    );
                }

                return Result.Success<IEnumerable<T>>(result.Value.SelectMany(i => i), result.Remainder);
            };
        }

        /// <summary>
        /// Attempt parsing only if all of the parsers in <paramref name="parsers"/> fail
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="this">The parser.</param>
        /// <param name="parsers">The other except parsers.</param>
        /// <returns>A new parser that will only succeed if the subsequent parsers fail</returns>
        public static Parser<TResult> ExceptAny<TResult>(this Parser<TResult> @this, params Parser<dynamic>[] parsers)
        {
            return input =>
            {
                var anyResult = parsers.Any(t => t.Invoke(input).WasSuccessful);
                if (anyResult)
                {
                    return Result.Failure<TResult>(
                        input,
                        string.Format("One of the excepted parsers succeeded"),
                        new[] { string.Empty }
                    );
                }

                return @this.Invoke(input);
            };
        }

        /// <summary>
        /// Implodes the <see cref="Parser{T}"/>'s <see cref="IEnumerable{T}"/> with the specified aggregator
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="this">The parser.</param>
        /// <param name="seed">The seed value.</param>
        /// <param name="accumulator">The accumulator function.</param>
        /// <returns>A <see cref="Parser{T}"/> that will aggregate the results afterwards</returns>
        public static Parser<TResult> Implode<TInput, TResult>(
            this Parser<IEnumerable<TInput>> @this,
            TResult seed,
            Func<TResult, TInput, TResult> accumulator
        )
        {
            return input =>
            {
                var result = @this.Invoke(input);
                if (result.WasSuccessful == false)
                {
                    return Result.Failure<TResult>(
                        result.Remainder,
                        string.Format("Failure to implode. Dependent parser failed. {0}", result.Message), 
                        result.Expectations
                    );
                }
                return Result.Success<TResult>(result.Value.Aggregate<TInput, TResult>(seed, accumulator), result.Remainder);
            };
        }

        /// <summary>
        /// Parses the antecedent parser and feeds its results into the <paramref name="continuation" /> parser
        /// 
        /// TODO: WRITE UTS
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="this">The antecedent parser</param>
        /// <param name="converter">The converter function to link the two parsers together.</param>
        /// <param name="continuation">The continuation parser.</param>
        /// <returns>
        /// A new parser that is the parsing of the antecedent's result using the continuation parser
        /// </returns>
        public static Parser<TResult> ContinueWith<TInput, TResult>(
            this Parser<TInput> @this,
            Func<TInput, string> converter,
            Parser<TResult> continuation
        )
        {
            return input =>
            {
                var result = @this.Invoke(input);
                if (result.WasSuccessful == false)
                {
                    return Result.Failure<TResult>(
                        result.Remainder,
                        string.Format("Failure to continue with the continuation parser. The antecedent parser failed. {0}", result.Message),
                        result.Expectations
                    );
                }

                return continuation.Invoke(new Input(converter.Invoke(result.Value)));
            };
        }

        /// <summary>
        /// Parses the antecedent parser and feeds its results into the <paramref name="continuation" /> parser
        /// 
        /// TODO: UTs
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="this">The antecedent parser.</param>
        /// <param name="continuation">The continuation parser.</param>
        /// <returns>
        /// A new parser that is the parsing of the antecedent's result using the continuation parser
        /// </returns>
        public static Parser<TResult> ContinueWith<TResult>(
            this Parser<string> @this,
            Parser<TResult> continuation
        )
        {
            return @this.ContinueWith(i => i, continuation);
        }

        /// <summary>
        /// Returns a parser that will scan for a token that is parsable by the given parser
        /// 
        /// TODO: UTs
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="scanner">The parser.</param>
        /// <returns>A new parser that scans for any token that is parsable by the given parser</returns>
        public static Parser<TResult> ScanFor<TResult>(Parser<TResult> scanner)
        {
            return from _ in Parse.AnyChar.Except(scanner).Many().Text()
                   from s in scanner
                   select s;
        }

        /// <summary>
        /// Returns a new parser that will invoke this parser and then reset the input back to what it was when it
        /// was fed into this parser
        /// 
        /// TODO: UTs
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="this">The parser.</param>
        /// <returns>A new parser that resets its input on completion</returns>
        public static Parser<TResult> ResetInput<TResult>(this Parser<TResult> @this)
        {
            return input =>
            {
                var result = @this.Invoke(input);
                return result.WasSuccessful
                    ? Result.Success<TResult>(result.Value, input)
                    : Result.Failure<TResult>(input, result.Message, result.Expectations);
            };
        }

        /// <summary>
        /// Constructs a parser that indicates the given parser is optional. The only difference between the 
        /// traditional Parser{TResult}.Optional() construct is that this parser will return a Maybe{TResult} instead of
        /// an IOption{TResult}
        /// 
        /// TODO: UTs
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="this">The parser.</param>
        /// <returns>A new parser that returns a Maybe{TResult}</returns>
        public static Parser<Maybe<TResult>> OptionalMaybe<TResult>(this Parser<TResult> @this)
        {
            return from r in @this.Optional()
                   select r.ConvertFromIOptionToMaybe();
        }

        /// <summary>
        /// Memoizes the specified <see cref="Parser{T}"/>
        /// </summary>
        /// <typeparam name="TResult">The type of result</typeparam>
        /// <param name="this">The parser</param>
        /// <returns>A memoized version of this parser</returns>
        public static Parser<TResult> Memoize<TResult>(this Parser<TResult> @this)
        {
            var memopad = new ConcurrentDictionary<IInput, IResult<TResult>>();
            return input => memopad.GetOrAdd(input, @this.Invoke);
        }

        private static Maybe<int> TryParseInt(string input)
        {
            int output;
            if (int.TryParse(input, out output))
            {
                return output.ToMaybe();
            }

            return Maybe<int>.Nothing;
        }
    }
}
