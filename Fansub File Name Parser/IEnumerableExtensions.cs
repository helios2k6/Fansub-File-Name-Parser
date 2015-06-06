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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FansubFileNameParser
{
    /// <summary>
    /// Extension methods for the <see cref="IEnumerable{T}"/> class
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Returns a string by calling ToString() on each element and concatenating them together with a space inbetween
        /// </summary>
        /// <typeparam name="T">The type of element</typeparam>
        /// <param name="this">The <see cref="IEnumerable{T}"/></param>
        /// <returns>A string of the <see cref="IEnumerable{T}"/></returns>
        public static string ToStringEx<T>(this IEnumerable<T> @this)
        {
            if (@this == null)
            {
                throw new NullReferenceException("@this");
            }

            var builder = new StringBuilder();
            foreach (var t in @this)
            {
                builder.Append(t.ToString()).Append(" ");
            }

            return builder.ToString().Trim();
        }

        /// <summary>
        /// Gets the aggregated hashcode of each of the <see cref="IEnumerable{T}"/> by
        /// calling GetHashCode() on each element and XOR'ing it to seed value
        /// </summary>
        /// <typeparam name="T">The type of the element</typeparam>
        /// <param name="this">The <see cref="IEnumerable{T}"/></param>
        /// <returns>The hashcode</returns>
        public static int GetHashCodeEx<T>(this IEnumerable<T> @this)
        {
            if (@this == null)
            {
                throw new NullReferenceException("@this");
            }

            return @this.Aggregate(1, (acc, t) => acc ^= t.GetHashCode());
        }
    }
}
