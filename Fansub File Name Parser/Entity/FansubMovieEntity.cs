﻿/*
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
using System.Runtime.Serialization;

namespace FansubFileNameParser.Entity
{
    /// <summary>
    /// Represents a single anime movie that is represented as a file
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class FansubMovieEntity : FansubFileEntityBase, IEquatable<FansubMovieEntity>
    {
        #region private static fields
        private const string MovieNumberKey = "MovieNumber";
        private const string SubtitleKey = "Subtitle";
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="FansubMovieEntity"/> class.
        /// </summary>
        public FansubMovieEntity()
        {
            MovieNumber = Maybe<int>.Nothing;
            Subtitle = Maybe<string>.Nothing;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FansubMovieEntity"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        private FansubMovieEntity(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            MovieNumber = MaybeExtensions.GetValueNullableMaybe<int>(info, MovieNumberKey);
            Subtitle = info.GetString(SubtitleKey).ToMaybe();
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets or sets the movie number.
        /// </summary>
        /// <value>
        /// The movie number.
        /// </value>
        [JsonProperty(PropertyName = "MovieNumber")]
        public Maybe<int> MovieNumber { get; set; }

        /// <summary>
        /// Gets or sets the movie subtitle.
        /// </summary>
        /// <value>
        /// The movie subtitle.
        /// </value>
        [JsonProperty(PropertyName = "Subtitle")]
        public Maybe<string> Subtitle { get; set; }
        #endregion

        #region public methods
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "{0} [Movie Number = {1}] [Subtitle = {2}]",
                base.ToString(),
                MovieNumber,
                Subtitle
            );
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(FansubMovieEntity other)
        {
            if (EqualsPreamble(other) == false)
            {
                return false;
            }

            return base.Equals(other)
                && MovieNumber.Equals(other.MovieNumber)
                && Subtitle.Equals(other.Subtitle);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object other)
        {
            return Equals(other as FansubMovieEntity);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode()
                ^ MovieNumber.GetHashCode()
                ^ Subtitle.GetHashCode();
        }

        /// <summary>
        /// Accept the specified visitor
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Accept(IFansubEntityVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Gets the object data for this class' hierarchy
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(MovieNumberKey, MovieNumber.ToNullable());
            info.AddValue(SubtitleKey, Subtitle.OrElseDefault());

            base.GetObjectData(info, context);
        }
        #endregion

        #region private methods
        private bool EqualsPreamble(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;

            return true;
        }
        #endregion
    }
}
