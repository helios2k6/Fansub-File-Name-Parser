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
using Sprache;
using System;

namespace FansubFileNameParser.Entity
{
    /// <summary>
    /// A static class of Parser{string} objects that parse entity specific strings
    /// </summary>
    internal static class EntityParsers
    {
        #region public nested class
        /// <summary>
        /// The result of an OP/ED Parser
        /// </summary>
        public sealed class OPEDParseResult : IEquatable<OPEDParseResult>
        {
            /// <summary>
            /// Gets or sets the creditless prefix.
            /// </summary>
            /// <value>
            /// The creditless prefix.
            /// </value>
            public Maybe<string> CreditlessPrefix { get; set; }
            /// <summary>
            /// Gets or sets the Opening or Ending Token
            /// </summary>
            /// <value>
            /// The Opening or Ending Token
            /// </value>
            public Maybe<string> OPEDToken { get; set; }
            /// <summary>
            /// Gets or sets the sequence number.
            /// </summary>
            /// <value>
            /// The sequence number.
            /// </value>
            public Maybe<int> SequenceNumber { get; set; }

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String" /> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                return string.Format("[{0}] [{1}] [{2}]", CreditlessPrefix, OPEDToken, SequenceNumber);
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
                return Equals(other as OPEDParseResult);
            }

            /// <summary>
            /// Indicates whether the current object is equal to another object of the same type.
            /// </summary>
            /// <param name="other">An object to compare with this object.</param>
            /// <returns>
            /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
            /// </returns>
            public bool Equals(OPEDParseResult other)
            {
                if (EqualsPreamble(other) == false)
                {
                    return false;
                }

                return CreditlessPrefix.Equals(other.CreditlessPrefix)
                    && OPEDToken.Equals(other.OPEDToken)
                    && SequenceNumber.Equals(other.SequenceNumber);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public override int GetHashCode()
            {
                return CreditlessPrefix.GetHashCode()
                    ^ OPEDToken.GetHashCode()
                    ^ SequenceNumber.GetHashCode();
            }

            private bool EqualsPreamble(object other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                if (GetType() != other.GetType()) return false;

                return true;
            }

        }
        #endregion
        #region Base
        #endregion
        #region Directory
        private static readonly Parser<string> Volume = Parse.IgnoreCase("VOLUME").Text();

        private static readonly Parser<string> Vol = Parse.IgnoreCase("VOL").Text();

        private static readonly Parser<string> VolumeToken = Volume.Or(Vol);

        private static readonly Parser<Tuple<int, int>> EpisodeRange =
            from firstNumber in ExtraParsers.Int
            from _ in BaseGrammars.Dash
            from secondNumber in ExtraParsers.Int
            select Tuple.Create(firstNumber, secondNumber);
        #endregion
        #region OP / ED
        #region Parsers
        private static readonly Parser<string> OP = Parse.IgnoreCase("OP").Text();

        private static readonly Parser<string> Opening = Parse.IgnoreCase("OPENING").Text();

        private static readonly Parser<string> OpeningToken = OP.Or(Opening);

        private static readonly Parser<string> ED = Parse.IgnoreCase("ED").Text();

        private static readonly Parser<string> Ending = Parse.IgnoreCase("ENDING").Text();

        private static readonly Parser<string> EndingToken = ED.Or(Ending).Text();
        #endregion
        #region Creditless Parsers
        private static readonly Parser<string> NC = Parse.IgnoreCase("NC").Text();

        private static readonly Parser<string> Creditless = Parse.IgnoreCase("CREDITLESS").Text();

        private static readonly Parser<string> NonCredit = Parse.IgnoreCase("NONCREDIT").Text();

        private static readonly Parser<string> NonDashCredit = Parse.IgnoreCase("NON-CREDIT").Text();

        private static readonly Parser<string> CreditlessToken = NC.Or(Creditless).Or(NonCredit).Or(NonDashCredit);
        #endregion
        #region Composite Parsers
        private static readonly Parser<OPEDParseResult> AnyOpeningToken =
            from creditPrefix in CreditlessToken.Optional()
            from spaceBetweenPrefixAndOP in Parse.WhiteSpace.Many().Optional()
            from openingToken in OpeningToken
            from spaceBetweenOPAndNumber in Parse.WhiteSpace.Many().Optional()
            from sequenceNumber in ExtraParsers.Int.Optional()
            select new OPEDParseResult
            {
                CreditlessPrefix = creditPrefix.ConvertFromIOptionToMaybe(),
                OPEDToken = openingToken.ToMaybe(),
                SequenceNumber = sequenceNumber.ConvertFromIOptionToMaybe(),
            };

        private static readonly Parser<OPEDParseResult> AnyEndingToken =
            from creditPrefix in CreditlessToken.Optional()
            from spaceBetweenPrefixAndED in Parse.WhiteSpace.Many().Optional()
            from endingToken in EndingToken
            from spaceBetweenEDAndNumber in Parse.WhiteSpace.Many().Optional()
            from sequenceNumber in ExtraParsers.Int.Optional()
            select new OPEDParseResult
            {
                CreditlessPrefix = creditPrefix.ConvertFromIOptionToMaybe(),
                OPEDToken = endingToken.ToMaybe(),
                SequenceNumber = sequenceNumber.ConvertFromIOptionToMaybe(),
            };

        /// <summary>
        /// Parses the OP embedded in any amount of text
        /// </summary>
        public static readonly Parser<OPEDParseResult> ParseOpeningFromLine =
            from contentBefore in Parse.AnyChar.Except(AnyOpeningToken).Many().Optional()
            from openingToken in AnyOpeningToken
            select openingToken;

        /// <summary>
        /// Parses the ED embedded in any amount of text 
        /// </summary>
        public static readonly Parser<OPEDParseResult> ParseEndingFromLine =
            from contentBefore in Parse.AnyChar.Except(AnyEndingToken).Many().Optional()
            from endingToken in AnyEndingToken
            select endingToken;
        #endregion
        #endregion
    }
}
