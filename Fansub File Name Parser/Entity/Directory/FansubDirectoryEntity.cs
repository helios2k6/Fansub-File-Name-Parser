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

namespace FansubFileNameParser.Entity.Directory
{
    /// <summary>
    /// Represents a directory of anime episodes and other materials.  
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class FansubDirectoryEntity : FansubEntityBase, IEquatable<FansubDirectoryEntity>
    {
        #region private static readonly fields
        private const string VolumeKey = "Volume";
        private const string EpisodeRangeKey = "EpisodeRange";
        private const string MediaMetadataKey = "MediaMetadata";
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="FansubDirectoryEntity"/> class.
        /// </summary>
        public FansubDirectoryEntity()
        {
            Volume = Maybe<int>.Nothing;
            EpisodeRange = Maybe<Tuple<int, int>>.Nothing;
        }

        private FansubDirectoryEntity(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Volume = MaybeExtensions.GetValueNullableMaybe<int>(info, VolumeKey);
            EpisodeRange = ((Tuple<int, int>)info.GetValue(EpisodeRangeKey, typeof(Tuple<int, int>))).ToMaybe();
            MediaMetadata = ((MediaMetadata)info.GetValue(MediaMetadataKey, typeof(MediaMetadata))).ToMaybe();
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets or sets the BD/DVD volume number.
        /// </summary>
        /// <value>The volume number.</value>
        [JsonProperty(PropertyName = "Volume")]
        public Maybe<int> Volume { get; set; }

        /// <summary>
        /// Gets or sets the episode range.
        /// </summary>
        /// <value>The episode range.</value>
        [JsonProperty(PropertyName = "EpisodeRange")]
        public Maybe<Tuple<int, int>> EpisodeRange { get; set; }

        /// <summary>
        /// Gets or sets the media metadata.
        /// </summary>
        /// <value>
        /// The media metadata.
        /// </value>
        [JsonProperty(PropertyName = "MediaMetadata")]
        public Maybe<MediaMetadata> MediaMetadata { get; set; }
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
                "{0} [Volume = {1}] [Episode Range = {2}] [Media Metadata = {3}]",
                base.ToString(),
                Volume,
                EpisodeRange,
                MediaMetadata
            );
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(FansubDirectoryEntity other)
        {
            if (EqualsPreamble(other) == false)
            {
                return false;
            }

            return base.Equals(other)
                && Volume.Equals(other.Volume)
                && EpisodeRange.Equals(other.EpisodeRange)
                && MediaMetadata.Equals(other.MediaMetadata);
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
            return Equals(other as FansubDirectoryEntity);
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
                ^ Volume.GetHashCode()
                ^ EpisodeRange.GetHashCode()
                ^ MediaMetadata.GetHashCode();
        }

        /// <summary>
        /// Gets the object data for this class' hierarchy
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(VolumeKey, Volume.ToNullable());
            info.AddValue(EpisodeRangeKey, EpisodeRange.OrElseDefault());
            info.AddValue(MediaMetadataKey, MediaMetadata.OrElseDefault());

            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Accept the specified visitor
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Accept(IFansubEntityVisitor visitor)
        {
            visitor.Visit(this);
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
