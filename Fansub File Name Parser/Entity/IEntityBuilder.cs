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

namespace FansubFileNameParser.Entity
{
    /// <summary>
    /// An object that can build a specific <see cref="IFansubEntity" />
    /// </summary>
    /// <typeparam name="T">The <see cref="IFansubEntity"/> this builder makes</typeparam>
    internal interface IEntityBuilder<T> where T : IFansubEntity
    {
        /// <summary>
        /// Sets the input string that needs to be parsed in order to build this entity
        /// </summary>
        /// <param name="inputString">The input string.</param>
        void SetInputString(string inputString);

        /// <summary>
        /// Builds the <see cref="IFansubEntity"/> with the given string
        /// </summary>
        /// <returns>The constructed <see cref="IFansubEntity"/></returns>
        T Build();
    }
}
