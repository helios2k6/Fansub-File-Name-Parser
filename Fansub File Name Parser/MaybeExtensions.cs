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
using System.Runtime.Serialization;

namespace FansubFileNameParser
{
    /// <summary>
    /// Extends the <see cref="Maybe{T}"/> class with convenient methods 
    /// </summary>
    public static class MaybeExtensions
    {
        /// <summary>
        /// Gets to string for an <see cref="Enum"/> wrapped by a <see cref="Maybe{T}"/>
        /// </summary>
        /// <typeparam name="T">The Enum</typeparam>
        /// <param name="maybe">The Maybe wrapper</param>
        /// <returns>The string representation of the Enum</returns>
        public static string ToStringEnum<T>(this Maybe<T> maybe) where T : struct
        {
            return maybe.SelectOrElse(t => Enum.GetName(typeof(T), t), () => Maybe<T>.Nothing.ToString());
        }

        /// <summary>
        /// Gets the value of a <see cref="Nullable{T}"/> serialized in a <see cref="SerializationInfo"/> object and wraps it in a <see cref="Maybe{T}"/>
        /// </summary>
        /// <typeparam name="T">The Nullable type</typeparam>
        /// <param name="info">The serialized form of the Nullable</param>
        /// <param name="key">The serialization key</param>
        /// <returns>A Maybe wrapped Nullable value</returns>
        public static Maybe<T> GetValueNullableMaybe<T>(SerializationInfo info, string key) where T : struct
        {
            return ((T?)info.GetValue(key, typeof(T?))).ToMaybe();
        }
    }
}
