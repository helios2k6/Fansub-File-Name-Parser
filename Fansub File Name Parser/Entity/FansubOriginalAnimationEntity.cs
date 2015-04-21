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

using FansubFileNameParser;
using Functional.Maybe;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace FansubFileNameParser.Entity
{
    /// <summary>
    /// Represents a single anime OVA or ONA that is represented by a file
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class FansubOriginalAnimationEntity : FansubFileEntityBase, IEquatable<FansubOriginalAnimationEntity>
    {
        #region internal enums
        /// <summary>
        /// Represents the origin of this OVA/ONA/OAD
        /// </summary>
        public enum ReleaseType
        {
            /// <summary>
            /// Represents an Original Video Animation, which may come from a DVD, a net release, or a BD
            /// </summary>
            OVA,
            /// <summary>
            /// Represents an Original Net Animation, which only comes from a net release
            /// </summary>
            ONA,
            /// <summary>
            /// Represents an Original Animation DVD, which only comes from a DVD
            /// </summary>
            OAD,
        }
        #endregion

        #region private static fields
        private const string EpisodeNumberKey = "EpisodeNumber";
        private const string TypeKey = "Type";
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="FansubOriginalAnimationEntity"/> class.
        /// </summary>
        public FansubOriginalAnimationEntity()
        {
            EpisodeNumber = Maybe<int>.Nothing;
            Type = Maybe<ReleaseType>.Nothing;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FansubOriginalAnimationEntity"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        private FansubOriginalAnimationEntity(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            EpisodeNumber = MaybeExtensions.GetValueNullableMaybe<int>(info, EpisodeNumberKey);
            Type = MaybeExtensions.GetValueNullableMaybe<ReleaseType>(info, TypeKey);
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets or sets the episode number.
        /// </summary>
        /// <value>
        /// The episode number.
        /// </value>
        [JsonProperty(PropertyName = "EpisodeNumber")]
        public Maybe<int> EpisodeNumber { get; set; }

        /// <summary>
        /// Gets or sets the type of the release.
        /// </summary>
        /// <value>
        /// The type of the release.
        /// </value>
        [JsonProperty(PropertyName = "Type")]
        public Maybe<ReleaseType> Type { get; set; }
        #endregion

        #region public methods
        /// <summary>
        /// Accept the specified visitor
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Accept(IFansubEntityVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} [Episode Number = {1}] [Release Type = {2}]", 
                base.ToString(),
                EpisodeNumber,
                Type.ToStringEnum());
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(FansubOriginalAnimationEntity other)
        {
            if (EqualsPreamble(other) == false)
            {
                return false;
            }

            return base.Equals(other)
                && EpisodeNumber.Equals(other.EpisodeNumber)
                && Type.Equals(other.Type);
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
            return Equals(other as FansubOriginalAnimationEntity);
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
                ^ EpisodeNumber.GetHashCode()
                ^ Type.GetHashCode();
        }

        /// <summary>
        /// Gets the object data for this class' hierarchy
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(EpisodeNumberKey, EpisodeNumber.ToNullable());
            info.AddValue(TypeKey, Type.ToNullable());

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
