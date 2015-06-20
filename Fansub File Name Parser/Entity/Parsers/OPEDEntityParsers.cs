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
using Sprache;

namespace FansubFileNameParser.Entity.Parsers
{
    /// <summary>
    /// Contains the OP/ED Parser
    /// </summary>
    internal static class OPEDEntityParsers
    {
        #region private classes
        /// <summary>
        /// The result of an OP/ED Parser
        /// </summary>
        private sealed class OPEDParseResult
        {
            public Maybe<string> CreditlessPrefix { get; set; }
            public Maybe<string> OPEDToken { get; set; }
            public Maybe<int> SequenceNumber { get; set; }
        }
        #endregion
        #region private fields
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

        private static readonly Parser<string> CreditlessToken =
            NC.Or(Creditless).Or(NonCredit).Or(NonDashCredit).Memoize();
        #endregion
        #region Composite Parsers
        private static readonly Parser<OPEDParseResult> AnyOpeningToken =
           from creditPrefix in CreditlessToken.OptionalMaybe()
           from _ in Parse.WhiteSpace.Many()
           from openingToken in OpeningToken
           from __ in Parse.WhiteSpace.Many()
           from sequenceNumber in ExtraParsers.Int.OptionalMaybe()
           select new OPEDParseResult
           {
               CreditlessPrefix = creditPrefix,
               OPEDToken = openingToken.ToMaybe(),
               SequenceNumber = sequenceNumber,
           };

        private static readonly Parser<OPEDParseResult> AnyEndingToken =
            from creditPrefix in CreditlessToken.OptionalMaybe()
            from _ in Parse.WhiteSpace.Many()
            from endingToken in EndingToken
            from __ in Parse.WhiteSpace.Many()
            from sequenceNumber in ExtraParsers.Int.OptionalMaybe()
            select new OPEDParseResult
            {
                CreditlessPrefix = creditPrefix,
                OPEDToken = endingToken.ToMaybe(),
                SequenceNumber = sequenceNumber,
            };

        private static readonly Parser<IFansubEntity> ParseOpening =
            from metadata in BaseEntityParsers.MediaMetadata.OptionalMaybe().ResetInput()
            from fansubGroup in BaseEntityParsers.FansubGroup.OptionalMaybe().ResetInput()
            from series in BaseEntityParsers.SeriesName.OptionalMaybe().ResetInput()
            from extension in FileEntityParsers.FileExtension.OptionalMaybe().ResetInput()
            from openingToken in ExtraParsers.ScanFor(AnyOpeningToken)
            select new FansubOPEDEntity
            {
                Group = fansubGroup,
                Series = series,
                Metadata = metadata,
                Extension = extension,
                SequenceNumber = openingToken.SequenceNumber,
                Part = FansubOPEDEntity.Segment.OP.ToMaybe(),
                NoCredits = openingToken.CreditlessPrefix.HasValue,
            };

        private static readonly Parser<IFansubEntity> ParseEnding =
            from metadata in BaseEntityParsers.MediaMetadata.OptionalMaybe().ResetInput()
            from fansubGroup in BaseEntityParsers.FansubGroup.OptionalMaybe().ResetInput()
            from series in BaseEntityParsers.SeriesName.OptionalMaybe().ResetInput()
            from extension in FileEntityParsers.FileExtension.OptionalMaybe().ResetInput()
            from openingToken in ExtraParsers.ScanFor(AnyEndingToken)
            select new FansubOPEDEntity
            {
                Group = fansubGroup,
                Series = series,
                Metadata = metadata,
                Extension = extension,
                SequenceNumber = openingToken.SequenceNumber,
                Part = FansubOPEDEntity.Segment.ED.ToMaybe(),
                NoCredits = openingToken.CreditlessPrefix.HasValue,
            };

        private static readonly Parser<IFansubEntity> OpeningOrEndingParser = ParseOpening.Or(ParseEnding).Memoize();
        #endregion
        #endregion
        #region public properies
        /// <summary>
        /// Gets the OP/ED parser
        /// </summary>
        /// <value>
        /// The OP/ED parser
        /// </value>
        public static Parser<IFansubEntity> OpeningOrEnding
        {
            get { return OpeningOrEndingParser; }
        }
        #endregion
    }
}
