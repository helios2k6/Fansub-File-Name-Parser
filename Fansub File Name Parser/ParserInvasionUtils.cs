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

using Sprache;

namespace FansubFileNameParser
{
    /// <summary>
    /// Invasive tools that can be used to debug troublesome Parsers
    /// </summary>
    public static class ParserInvasionUtils
    {
        /// <summary>
        /// A simple aspect interception tool to help debug and determine what is happening during a parse
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="this"></param>
        /// <param name="token">Marker token</param>
        /// <returns></returns>
        public static Parser<TResult> Intercept<TResult>(
            this Parser<TResult> @this,
            string token
        )
        {
            return input =>
            {
                var savedToken = token;
                var result = @this.Invoke(input);
                return result;
            };
        }
    }
}
