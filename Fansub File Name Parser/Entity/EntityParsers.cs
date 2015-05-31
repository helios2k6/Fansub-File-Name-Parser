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
using Sprache;
using System.Linq;

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
        public sealed class OPEDParseResult
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
        }
        #endregion
        #region OP / ED Parsers
        private static readonly Parser<string> OP = Parse.IgnoreCase("OP").Text();

        private static readonly Parser<string> NCOP = Parse.IgnoreCase("NCOP").Text();

        private static readonly Parser<string> Opening = Parse.IgnoreCase("OPENING").Text();

        private static readonly Parser<string> OpeningToken = OP.Or(NCOP).Or(Opening);

        private static readonly Parser<string> ED = Parse.IgnoreCase("ED").Text();

        private static readonly Parser<string> NCED = Parse.IgnoreCase("NCED").Text();

        private static readonly Parser<string> Ending = Parse.IgnoreCase("ENDING").Text();

        private static readonly Parser<string> EndingToken = ED.Or(NCED).Or(Ending).Text();
        #endregion
        #region Creditless Parsers
        private static readonly Parser<string> Creditless = Parse.IgnoreCase("CREDITLESS").Text();

        private static readonly Parser<string> NonCredit = Parse.IgnoreCase("NONCREDIT").Text();

        private static readonly Parser<string> NonDashCredit = Parse.IgnoreCase("NON-CREDIT").Text();

        private static readonly Parser<string> NonCreditPrefix = Creditless.Or(NonCredit).Or(NonDashCredit);
        #endregion
        #region Composite Parsers
        private static readonly Parser<OPEDParseResult> AnyOpeningToken =
            from creditPrefix in NonCreditPrefix.Optional()
            from openingToken in OpeningToken
            from possibleSpace in Parse.WhiteSpace.Many().Optional()
            from sequenceNumber in ExtraParsers.Int.Optional()
            select new OPEDParseResult
            {
                CreditlessPrefix = creditPrefix.ConvertFromIOptionToMaybe(),
                OPEDToken = openingToken.ToMaybe(),
                SequenceNumber = sequenceNumber.ConvertFromIOptionToMaybe(),
            };

        private static readonly Parser<OPEDParseResult> AnyEndingToken =
            from creditPrefix in NonCreditPrefix.Optional()
            from endingToken in EndingToken
            from possibleSpace in Parse.WhiteSpace.Many().Optional()
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
            from contentBefore in Parse.AnyChar.Many().Except(AnyOpeningToken).Optional()
            from openingToken in AnyOpeningToken
            select openingToken;

        /// <summary>
        /// Parses the ED embedded in any amount of text 
        /// </summary>
        public static readonly Parser<OPEDParseResult> ParseEndingFromLine =
            from contentBefore in Parse.AnyChar.Many().Except(AnyOpeningToken).Optional()
            from endingToken in AnyEndingToken
            select endingToken;
        #endregion
    }
}
