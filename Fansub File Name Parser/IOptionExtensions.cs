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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FansubFileNameParser
{
    /// <summary>
    /// Extension methods to the <seealso cref="IOption{T}"/> interface
    /// </summary>
    public static class IOptionExtensions
    {
        /// <summary>
        /// Converts an <seealso cref="IOption{T}"/> to a <seealso cref="Maybe{T}"/>
        /// </summary>
        /// <returns>The <seealso cref="Maybe{T}"/> representing the <seealso cref="IOption{T}"/></returns>
        /// <param name="this">The <seealso cref="IOption{T}"/></param>
        /// <typeparam name="T">The type wrapped by the <seealso cref="IOption{T}"/></typeparam>
        public static Maybe<T> ConvertFromIOptionToMaybe<T>(this IOption<T> @this)
        {
            if (@this.IsDefined)
            {
                return @this.Get().ToMaybe();
            }

            return Maybe<T>.Nothing;
        }

        /// <summary>
        /// Concatenates IOption{string} results
        /// </summary>
        /// <param name="this">The starting IOption{string}.</param>
        /// <param name="optionalStrings">The other IOption{string} objects</param>
        /// <returns>A string of all of the concatenated strings, or an empty string</returns>
        public static string Concat(this IOption<string> @this, params IOption<string>[] optionalStrings)
        {
            var builder = new StringBuilder();
            if (@this.IsDefined)
            {
                builder.Append(@this.Get());
            }

            foreach (var s in optionalStrings)
            {
                if (s.IsDefined)
                {
                    builder.Append(" ").Append(s.Get());
                }
            }

            return builder.ToString();
        }
    }
}

