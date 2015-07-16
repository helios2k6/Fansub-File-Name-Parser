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

        private static Maybe<int> TryParseInt(string input)
        {
            int output;
            if (int.TryParse(input, out output))
            {
                return output.ToMaybe();
            }

            return Maybe<int>.Nothing;
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
        /// Returns a parser that will cut out the first occurrence of the token that satisfies
        /// the given parser.
        /// 
        /// TODO: UNIT TEST
        /// </summary>
        /// <typeparam name="TResult">The result this parser returns</typeparam>
        /// <param name="cutter">The parser to use to cut out the token</param>
        /// <returns>A new parser that removes the unwanted token</returns>
        public static Parser<string> CutOut<TResult>(Parser<TResult> cutter)
        {
            return from front in Parse.AnyChar.Except(cutter).Many().Text()
                   from chomp in cutter
                   from end in Parse.AnyChar.Many().Text()
                   select string.Format("{0} {1}", front, end).Trim();
        }

        /// <summary>
        /// Returns a parser that will attempt to cut out the first occurance of the token that
        /// satisfies the given parser, or it will return the original input.
        /// 
        /// TODO: UNIT TEST
        /// </summary>
        /// <typeparam name="TResult">The result this parser returns</typeparam>
        /// <param name="cutter">The parser to use to cut out the token</param>
        /// <returns>A new parser that removes the unwanted token or returns the original input</returns>
        public static Parser<string> CutOutOrAllInput<TResult>(Parser<TResult> cutter)
        {
            return CutOut<TResult>(cutter).Or(Parse.AnyChar.Many().Text());
        }

        /// <summary>
        /// Cycles through each parser in the IEnumerable{Parser{TResult}} and will return
        /// the result of the first one that successfully parses the input string
        /// 
        /// TODO: UNIT TEST
        /// </summary>
        /// <typeparam name="TResult">The type of result to parse</typeparam>
        /// <param name="parsers">The IEnumerable of parsers</param>
        /// <returns>A single parser that will enumerate all of the given parsers</returns>
        public static Parser<TResult> Any<TResult>(IEnumerable<Parser<TResult>> parsers)
        {
            return input =>
            {
                foreach (var parser in parsers)
                {
                    var result = parser.Invoke(input);
                    if (result.WasSuccessful)
                    {
                        return result;
                    }
                }

                return Result.Failure<TResult>(
                    input,
                    "None of the provided parsers successfully parsed the input",
                    new string[0]
                );
            };
        }

        /// <summary>
        /// Collects the string leading up to a token
        /// </summary>
        /// <typeparam name="TThrowAway">The type of result to throw away.</typeparam>
        /// <param name="excepter">The excepter parser.</param>
        /// <returns>A new parser that collects the string all the way up to a specific point</returns>
        public static Parser<string> CollectExcept<TThrowAway>(Parser<TThrowAway> excepter)
        {
            return Parse.AnyChar.Except(excepter).Many().Text().Select(t => t.Trim());
        }

        /// <summary>
        /// Returns a new parser that will invoke this parser and then reset the input back to what it was when it
        /// was fed into this parser
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
        /// Constructs a parser that will only succeed if it consumes the last possible token that satisfies this 
        /// parser
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="this">The parser</param>
        /// <returns>A new parser that will succeed only if it parses the last token that satisfies this parser</returns>
        public static Parser<TResult> Last<TResult>(this Parser<TResult> @this)
        {
            return from f in @this
                   from _ in ExtraParsers.ScanFor(@this).Not()
                   select f;
        }

        /// <summary>
        /// Constructs a filtering parser that removes anything the given parser successfully parses.
        /// This parser consumes all characters
        /// </summary>
        /// <param name="filter">The filter parser</param>
        /// <returns>A newly constructed parser that filters tokens</returns>
        public static Parser<string> Filter<TResult>(Parser<TResult> filter)
        {
            var innerLoop = from mainToken in Parse.AnyChar.Except(filter).Many().Text()
                            from _ in filter.Optional()
                            select mainToken.Trim();

            return innerLoop.Many().Implode(
                string.Empty,
                (acc, i) => string.IsNullOrWhiteSpace(acc)
                    ? i
                    : string.Format("{0} {1}", acc, i)
            );
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

        /// <summary>
        /// Converts a Parser into a bool, mapping a successful parse to "true" and
        /// any other result to "false." 
        /// </summary>
        /// <remarks>
        /// This function ALWAYS succeeds, just like Optional{T}, but it simply maps the
        /// optional result to a true or false. This parser will consume input exactly like
        /// any Optional{T} parser would; it consumes the necessary input upon success and
        /// no input on failure.
        /// </remarks>
        /// <typeparam name="TResult">The type of the result from the Parser</typeparam>
        /// <param name="this">The Parser</param>
        /// <returns>A new parser that returns true or false depending on the parse result</returns>
        public static Parser<bool> WasSuccessful<TResult>(this Parser<TResult> @this)
        {
            return input =>
            {
                var result = @this.Invoke(input);
                if (result.WasSuccessful)
                {
                    return Result.Success<bool>(true, result.Remainder);
                }

                return Result.Success<bool>(false, input);
            };
        }

        /// <summary>
        /// Invokes this parser and upon success will set the remainder string to whatever
        /// this parser returned.
        /// 
        /// On failure, this will return the result of the failed parse
        /// </summary>
        /// <param name="this">The parser.</param>
        /// <returns>A new parser that sets the remainder as the value</returns>
        public static Parser<string> SetResultAsRemainder(this Parser<string> @this)
        {
            return input =>
            {
                var result = @this.Invoke(input);
                if (result.WasSuccessful == false)
                {
                    return result;
                }

                return Result.Success<string>(result.Value, new Input(result.Value));
            };
        }
    }
}
