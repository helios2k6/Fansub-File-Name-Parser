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
using System.Runtime.Serialization;

namespace FansubFileNameParser.Entity
{
    /// <summary>
    /// Represents a single OP (Opening) or ED (Ending) of an anime that is represented as a file
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class FansubOPEDEntity : FansubFileEntityBase, IEquatable<FansubOPEDEntity>, ISerializable
    {
        #region private enums
        /// <summary>
        /// Designates this as either the OP or the ED
        /// </summary>
        public enum Segment 
        { 
            /// <summary>
            /// Represents the Opening
            /// </summary>
            OP,

            /// <summary>
            /// Represents the Ending
            /// </summary>
            ED,

            /// <summary>
            /// Represents an unknown segment, which could be a repeated insert song
            /// </summary>
            Unknown,
        }
        #endregion

        #region private static fields
        private const string SequenceNumberKey = "SequenceNumber";
        private const string PartKey = "Part";
        private const string NoCreditsKey = "NoCredits";
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="FansubOPEDEntity"/> class.
        /// </summary>
        public FansubOPEDEntity()
        {
            SequenceNumber = Maybe<int>.Nothing;
            Part = Segment.Unknown;
            NoCredits = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FansubOPEDEntity"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        private FansubOPEDEntity(SerializationInfo info, StreamingContext context)
        {
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets or sets the OP/ED sequence number. The sequence number designates which OP/ED comes first, second, third, etc...
        /// </summary>
        /// <value>
        /// The sequence number
        /// </value>
        public Maybe<int> SequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the part. The part designates if this is an OP or an ED
        /// </summary>
        /// <value>
        /// The part.
        /// </value>
        public Segment Part { get; set; }

        /// <summary>
        /// Gets or sets whether this OP/ED has no credits
        /// </summary>
        /// <value>
        /// Whether this OP/ED has no credits
        /// </value>
        public bool NoCredits { get; set; }
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
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("");
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(FansubOPEDEntity other)
        {
            if (EqualsPreamble(other) == false)
            {
                return false;
            }

            return SequenceNumber.Equals(other.SequenceNumber)
                && NoCredits == other.NoCredits
                && Part == other.Part
                && FileMetadata.Equals(other.FileMetadata)
                && Extension.Equals(other.Extension)
                && Group.Equals(other.Group)
                && Series.Equals(other.Series);
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
            return Equals(other as FansubOPEDEntity);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return SequenceNumber.GetHashCode()
                ^ NoCredits.GetHashCode()
                ^ Part.GetHashCode()
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
