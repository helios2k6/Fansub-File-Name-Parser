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

        #region ctor
        #endregion

        #region public properties
        /// <summary>
        /// Gets or sets the episode number.
        /// </summary>
        /// <value>
        /// The episode number.
        /// </value>
        public Maybe<int> EpisodeNumber { get; set; }

        /// <summary>
        /// Gets or sets the type of the release.
        /// </summary>
        /// <value>
        /// The type of the release.
        /// </value>
        public Maybe<ReleaseType> Type { get; set; }
        #endregion

        #region public methods
        public override string ToString()
        {
            throw new System.NotImplementedException();
        }

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

        public override bool Equals(object other)
        {
            return Equals(other as FansubOriginalAnimationEntity);
        }

        public override int GetHashCode()
        {
            throw new System.NotImplementedException();
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
