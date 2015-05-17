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

namespace FansubFileNameParser.Entity
{
    /// <summary>
    /// The base class for all Fansub Entities
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class FansubEntityBase : IFansubEntity, IEquatable<FansubEntityBase>, ISerializable
    {
        #region private static fields
        private const string GroupKey = "Group";
        private const string SeriesKey = "Series";
        private const string MediaMetadataKey = "MediaMetadata";
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="FansubEntityBase"/> class.
        /// </summary>
        protected FansubEntityBase()
        {
            Group = Maybe<string>.Nothing;
            Series = Maybe<string>.Nothing;
            Metadata = Maybe<MediaMetadata>.Nothing;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FansubEntityBase"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected FansubEntityBase(SerializationInfo info, StreamingContext context)
        {
            Group = info.GetString(GroupKey).ToMaybe();
            Series = info.GetString(SeriesKey).ToMaybe();
            Metadata = ((MediaMetadata)info.GetValue(MediaMetadataKey, typeof(MediaMetadata))).ToMaybe();
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets or sets the fansub group name
        /// </summary>
        /// <value>
        /// The fansub group name
        /// </value>
        [JsonProperty(PropertyName = "Group")]
        public Maybe<string> Group { get; set; }

        /// <summary>
        /// Gets or sets the anime series name.
        /// </summary>
        /// <value>
        /// The anime series name
        /// </value>
        [JsonProperty(PropertyName = "Series")]
        public Maybe<string> Series { get; set; }

        /// <summary>
        /// Gets or sets the media metadata.
        /// </summary>
        /// <value>
        /// The media metadata.
        /// </value>
        [JsonProperty(PropertyName = "Metadata")]
        public Maybe<MediaMetadata> Metadata { get; set; }
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
            return string.Format("[Group = {0}] [Series = {1}] [Metadata = {2}]", Group, Series, Metadata);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(FansubEntityBase other)
        {
            if (EqualsPreamble(other) == false)
            {
                return false;
            }

            return Group.Equals(other.Group)
                && Series.Equals(other.Series)
                && Metadata.Equals(other.Metadata);
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
            return Equals(other as FansubEntityBase);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Group.GetHashCode()
                ^ Series.GetHashCode()
                ^ Metadata.GetHashCode();
        }

        /// <summary>
        /// Accept the specified visitor
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public abstract void Accept(IFansubEntityVisitor visitor);

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(GroupKey, Group.OrElseDefault());
            info.AddValue(SeriesKey, Series.OrElseDefault());
            info.AddValue(MediaMetadataKey, Metadata.OrElseDefault());
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
