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

using FansubFileNameParser.Metadata;
using Functional.Maybe;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace FansubFileNameParser.Entity
{
    /// <summary>
    /// Represents a single anime episode that is represented as a file
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class FansubEpisodeEntity : FansubFileEntityBase, IEquatable<FansubEpisodeEntity>, ISerializable
    {
        #region private static fields
        private const string EpisodeNumberKey = "EpisodeNumber";
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="FansubEpisodeEntity"/> class with default values
        /// </summary>
        public FansubEpisodeEntity()
        {
            EpisodeNumber = Maybe<int>.Nothing;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FansubEpisodeEntity"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        private FansubEpisodeEntity(SerializationInfo info, StreamingContext context)
        {
            EpisodeNumber = MaybeExtensions.GetValueNullableMaybe<int>(info, EpisodeNumberKey);
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets or sets the episode number.
        /// </summary>
        /// <value>
        /// The episode number.
        /// </value>
        public Maybe<int> EpisodeNumber { get; set; }
        #endregion

        #region public methods
        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(EpisodeNumberKey, EpisodeNumber.ToNullable());
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
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("[{0}] {1} ({2}).{3}", Group, Series, EpisodeNumber, Extension);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(FansubEpisodeEntity other)
        {
            if (EqualsPreamble(other) == false)
            {
                return false;
            }

            return FileMetadata.Equals(other.FileMetadata)
                && EpisodeNumber.Equals(other.EpisodeNumber);
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
            return Equals(other as FansubEpisodeEntity);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return EpisodeNumber.GetHashCode()
                ^ FileMetadata.GetHashCode()
                ^ Extension.GetHashCode()
                ^ Group.GetHashCode()
                ^ Series.GetHashCode();
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
