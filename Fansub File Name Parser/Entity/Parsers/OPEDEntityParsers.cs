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
using System.Collections.Generic;

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
            public bool CreditlessPrefix { get; set; }
            public Maybe<string> OPEDToken { get; set; }
            public Maybe<int> SequenceNumber { get; set; }
        }
        #endregion
        #region private fields
        private const string OPString = "OP";
        private const string OpeningString = "OPENING";
        private const string EDString = "ED";
        private const string EndingString = "ENDING";
        private const string NCString = "NC";
        private const string CreditlessString = "CREDITLESS";
        private const string NonCreditString = "NONCREDIT";
        private const string NonDashCreditString = "NON-CREDIT";
        private const string CleanString = "CLEAN";

        private static readonly ISet<string> InternalStrings = new HashSet<string>
        {
            OPString,
            OpeningString,
            EDString,
            EndingString,
            NCString,
            CreditlessString,
            NonCreditString,
            NonDashCreditString,
            CleanString,
        };
        #region Parsers
        private static readonly Parser<string> OP = Parse.IgnoreCase(OPString).Text();

        private static readonly Parser<string> Opening = Parse.IgnoreCase(OpeningString).Text();

        private static readonly Parser<string> OpeningToken = OP.Or(Opening);

        private static readonly Parser<string> ED = Parse.IgnoreCase(EDString).Text();

        private static readonly Parser<string> Ending = Parse.IgnoreCase(EndingString).Text();

        private static readonly Parser<string> EndingToken = ED.Or(Ending).Text();
        #endregion
        #region Creditless Parsers
        private static readonly Parser<string> NC = Parse.IgnoreCase(NCString).Text();

        private static readonly Parser<string> Creditless = Parse.IgnoreCase(CreditlessString).Text();

        private static readonly Parser<string> NonCredit = Parse.IgnoreCase(NonCreditString).Text();

        private static readonly Parser<string> NonDashCredit = Parse.IgnoreCase(NonDashCreditString).Text();

        private static readonly Parser<string> CreditlessToken =
            NC.Or(Creditless).Or(NonCredit).Or(NonDashCredit).Memoize();

        private static readonly Parser<string> Clean = Parse.IgnoreCase(CleanString).Text();

        private static readonly Parser<string> CleanInMetaTag = Clean.Contained(BaseGrammars.TagDeliminator, BaseGrammars.TagDeliminator);

        private static readonly Parser<string> AnyCleanToken = Clean.Or(CleanInMetaTag);
        #endregion
        #region Composite Parsers
        private static readonly Parser<OPEDParseResult> MainOpeningToken =
            from creditlessPrefix in CreditlessToken.WasSuccessful()
            from openingToken in OpeningToken.Token()
            from sequenceNumber in ExtraParsers.Int.OptionalMaybe()
            select new OPEDParseResult
            {
                CreditlessPrefix = creditlessPrefix,
                OPEDToken = openingToken.ToMaybe(),
                SequenceNumber = sequenceNumber,
            };

        private static readonly Parser<OPEDParseResult> MainEndingToken =
            from creditlessPrefix in CreditlessToken.WasSuccessful()
            from endingToken in EndingToken.Token()
            from sequenceNumber in ExtraParsers.Int.OptionalMaybe()
            select new OPEDParseResult
            {
                CreditlessPrefix = creditlessPrefix,
                OPEDToken = endingToken.ToMaybe(),
                SequenceNumber = sequenceNumber,
            };

        public static readonly Parser<string> TEST =
            from creditlessPrefix in CreditlessToken.WasSuccessful()
            from endingToken in EndingToken.Token()
            from sequenceNumber in ExtraParsers.Int.OptionalMaybe()
            select string.Format("SUCCESS");

        private static readonly Parser<IFansubEntity> ParseOpening =
            from cleanToken in ExtraParsers.ScanFor(AnyCleanToken).WasSuccessful().ResetInput()
            from metadata in BaseEntityParsers.MediaMetadata.OptionalMaybe().ResetInput()
            from fansubGroup in BaseEntityParsers.FansubGroup.OptionalMaybe().ResetInput()
            from series in BaseEntityParsers.SeriesName.OptionalMaybe().ResetInput()
            from extension in FileEntityParsers.FileExtension.OptionalMaybe().ResetInput()
            from _ in BaseGrammars.ContentBetweenTagGroups.SetResultAsRemainder()
            from openingToken in ExtraParsers.ScanFor(MainOpeningToken)
            select new FansubOPEDEntity
            {
                Group = fansubGroup,
                Series = FilterOutNonCreditsFromSeriesName(series),
                Metadata = metadata,
                Extension = extension,
                SequenceNumber = openingToken.SequenceNumber,
                Part = FansubOPEDEntity.Segment.OP.ToMaybe(),
                NoCredits = openingToken.CreditlessPrefix || cleanToken,
            };

        private static readonly Parser<IFansubEntity> ParseEnding =
            from cleanToken in ExtraParsers.ScanFor(AnyCleanToken).WasSuccessful().ResetInput()
            from metadata in BaseEntityParsers.MediaMetadata.OptionalMaybe().ResetInput()
            from fansubGroup in BaseEntityParsers.FansubGroup.OptionalMaybe().ResetInput()
            from series in BaseEntityParsers.SeriesName.OptionalMaybe().ResetInput()
            from extension in FileEntityParsers.FileExtension.OptionalMaybe().ResetInput()
            from endingToken in ExtraParsers.ScanFor(MainEndingToken)
            select new FansubOPEDEntity
            {
                Group = fansubGroup,
                Series = FilterOutNonCreditsFromSeriesName(series),
                Metadata = metadata,
                Extension = extension,
                SequenceNumber = endingToken.SequenceNumber,
                Part = FansubOPEDEntity.Segment.ED.ToMaybe(),
                NoCredits = endingToken.CreditlessPrefix || cleanToken,
            };

        private static readonly Parser<IFansubEntity> OpeningOrEndingParser = ParseOpening.Or(ParseEnding).Memoize();
        #endregion
        #endregion
        #region private methods
        private static Maybe<string> FilterOutNonCreditsFromSeriesName(Maybe<string> unfilteredSeriesName)
        {
            if (unfilteredSeriesName.HasValue == false)
            {
                return Maybe<string>.Nothing;
            }

            var filteredString = unfilteredSeriesName.Value;
            foreach (var internalString in InternalStrings)
            {
                filteredString = filteredString.Replace(internalString, string.Empty);
            }

            return filteredString.Trim().ToMaybe();
        }
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
