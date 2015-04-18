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
using Newtonsoft.Json;
using System;

namespace FansubFileNameParser.Entity
{
    /// <summary>
    /// The base class for all Fansub Entities
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class FansubEntityBase : IFansubEntity
    {
        #region public properties
        /// <summary>
        /// Gets or sets the fansub group name
        /// </summary>
        /// <value>
        /// The fansub group name
        /// </value>
        public Maybe<string> Group { get; set; }

        /// <summary>
        /// Gets or sets the anime series name.
        /// </summary>
        /// <value>
        /// The anime series name
        /// </value>
        public Maybe<string> Series { get; set; }
        #endregion

        #region public methods
        /// <summary>
        /// Accept the specified visitor
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public abstract void Accept(IFansubEntityVisitor visitor);
        #endregion
    }
}
