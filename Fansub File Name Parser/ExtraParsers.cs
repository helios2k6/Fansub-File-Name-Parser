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
                    var message = string.Format("Failure to implode. Dependent parser failed. {0}", result.Message);
                    return Result.Failure<IEnumerable<T>>(result.Remainder, message, result.Expectations);
                }

                return Result.Success<IEnumerable<T>>(result.Value.SelectMany(i => i), result.Remainder);
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
                    var message = string.Format("Failure to implode. Dependent parser failed. {0}", result.Message);
                    return Result.Failure<TResult>(result.Remainder, message, result.Expectations);
                }
                return Result.Success<TResult>(result.Value.Aggregate<TInput, TResult>(seed, accumulator), result.Remainder);
            };
        }

        /// <summary>
        /// Memoizes the specified <see cref="Parser{T}"/>
        /// </summary>
        /// <typeparam name="T">The type of result</typeparam>
        /// <param name="this">The parser</param>
        /// <returns>A memoized version of this parser</returns>
        public static Parser<T> Memoize<T>(this Parser<T> @this)
        {
            var memopad = new ConcurrentDictionary<IInput, IResult<T>>();
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
