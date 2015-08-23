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
using System.Collections.Generic;
using System.Linq;

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

        private static Lazy<IEnumerable<Parser<IFansubEntity>>> AllParsers =
            new Lazy<IEnumerable<Parser<IFansubEntity>>>(GenerateAllParsers);
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
            NC.Or(Creditless).Or(NonCredit).Or(NonDashCredit);

        private static readonly Parser<string> Clean = Parse.IgnoreCase(CleanString).Text();

        private static readonly Parser<string> CleanInMetaTag = Clean.Contained(BaseGrammars.TagDeliminator, BaseGrammars.TagDeliminator);

        private static readonly Parser<string> AnyCleanToken = Clean.Or(CleanInMetaTag);

        private static readonly Parser<int> VersionToken =
            from _ in Parse.IgnoreCase('V')
            from versionNo in ExtraParsers.Int
            select versionNo;
        #endregion
        #region Composite Parsers
        private static readonly Parser<OPEDParseResult> MainOpeningToken =
            from creditlessPrefix in CreditlessToken.WasSuccessful()
            from openingToken in OpeningToken.Token()
            from sequenceNumber in ExtraParsers.Int.OptionalMaybe()
            from version in VersionToken.OptionalMaybe()
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
            from version in VersionToken.OptionalMaybe()
            select new OPEDParseResult
            {
                CreditlessPrefix = creditlessPrefix,
                OPEDToken = endingToken.ToMaybe(),
                SequenceNumber = sequenceNumber,
            };

        private static readonly Parser<IFansubEntity> SeriesOpeningOrEndingParser =
            ExtraParsers.Any(AllParsers.Value).ConsumeAllRemainingInput();
        #endregion
        #endregion
        #region private methods
        private static IEnumerable<Parser<IFansubEntity>> GenerateAllParsers()
        {
            return GenerateOPParsers().Concat(GenerateEDParsers());
        }

        private static IEnumerable<Parser<IFansubEntity>> GenerateOPParsers()
        {
            return GenerateParserTemplates(MainOpeningToken, FansubOPEDEntity.Segment.OP.ToMaybe());
        }

        private static IEnumerable<Parser<IFansubEntity>> GenerateEDParsers()
        {
            return GenerateParserTemplates(MainEndingToken, FansubOPEDEntity.Segment.ED.ToMaybe());
        }

        private static IEnumerable<Parser<IFansubEntity>> GenerateParserTemplates(
            Parser<OPEDParseResult> tokenizer,
            Maybe<FansubOPEDEntity.Segment> segment
        )
        {
            var coreParser = (from metadata in BaseEntityParsers.MediaMetadata.OptionalMaybe().ResetInput()
                              from fansubGroup in BaseEntityParsers.FansubGroup.OptionalMaybe().ResetInput()
                              from series in BaseEntityParsers.SeriesName.OptionalMaybe().ResetInput()
                              from ext in FileEntityParsers.FileExtension.OptionalMaybe()
                              where series.HasValue
                              select new
                              {
                                  Metatdata = metadata,
                                  Group = fansubGroup,
                                  UnfilteredSeries = series,
                                  Extension = ext,
                              });

            // <series name> <dash> <token> (also checks metatags for "clean" tag)
            yield return from core in coreParser.ResetInput()
                         from _1 in BaseGrammars.MainContent.SetResultAsRemainder()
                         from _2 in BaseGrammars.LineUpToLastDashSeparatorToken
                         from _3 in BaseGrammars.DashSeparatorToken
                         from token in tokenizer
                         select new FansubOPEDEntity
                         {
                             Group = core.Group,
                             Series = core.UnfilteredSeries,
                             Metadata = core.Metatdata,
                             Extension = core.Extension,
                             SequenceNumber = token.SequenceNumber,
                             Part = segment,
                             NoCredits = token.CreditlessPrefix || CleanTokenFoundInMetadataTags(core.Metatdata),
                         };

            // <series name> <token> <extra content> (also checks metatags for "clean" tag)
            yield return from core in coreParser.ResetInput()
                         from _1 in BaseGrammars.MainContent.SetResultAsRemainder()
                         from token in ExtraParsers.ScanFor(tokenizer.Last())
                         select new FansubOPEDEntity
                         {
                             Group = core.Group,
                             Series = FilterOutNonCreditsFromSeriesName(core.UnfilteredSeries, tokenizer.Last()),
                             Metadata = core.Metatdata,
                             Extension = core.Extension,
                             SequenceNumber = token.SequenceNumber,
                             Part = segment,
                             NoCredits = token.CreditlessPrefix || CleanTokenFoundInMetadataTags(core.Metatdata)
                         };

            // <series name> (metadata tags contain information)
            yield return from core in coreParser
                         let parseResult = ScanThroughMetadataTags(
                            core.Group,
                            core.UnfilteredSeries,
                            core.Metatdata,
                            core.Extension,
                            tokenizer,
                            segment
                         )
                         where parseResult.HasValue
                         select parseResult.Value;
        }

        private static Maybe<FansubOPEDEntity> ScanThroughMetadataTags(
            Maybe<string> group,
            Maybe<string> series,
            Maybe<MediaMetadata> metadata,
            Maybe<string> extension,
            Parser<OPEDParseResult> tokenizer,
            Maybe<FansubOPEDEntity.Segment> segment
        )
        {
            if (metadata.HasValue == false)
            {
                return Maybe<FansubOPEDEntity>.Nothing;
            }

            var tagParser = ExtraParsers.ScanFor(tokenizer);
            var cleanToken = CleanTokenFoundInMetadataTags(metadata);
            foreach (var tag in metadata.Value.UnusedTags)
            {
                var tagParseResult = tagParser.TryParse(tag);
                if (tagParseResult.WasSuccessful)
                {
                    return new FansubOPEDEntity
                    {
                        Group = group,
                        Series = series,
                        Metadata = metadata,
                        Extension = extension,
                        Part = segment,
                        SequenceNumber = tagParseResult.Value.SequenceNumber,
                        NoCredits = cleanToken || tagParseResult.Value.CreditlessPrefix,
                    }.ToMaybe();
                }
            }

            return Maybe<FansubOPEDEntity>.Nothing;
        }

        private static bool CleanTokenFoundInMetadataTags(Maybe<MediaMetadata> mediaMetadata)
        {
            if (mediaMetadata.IsNothing())
            {
                return false;
            }

            var cleanTokenScanner = ExtraParsers.ScanFor(AnyCleanToken);
            return mediaMetadata.Value.UnusedTags.Any(tag => cleanTokenScanner.TryParse(tag).WasSuccessful);
        }

        private static Maybe<string> FilterOutNonCreditsFromSeriesName(
            Maybe<string> unfilteredSeriesName,
            Parser<OPEDParseResult> tokenizer
        )
        {
            if (unfilteredSeriesName.HasValue == false)
            {
                return Maybe<string>.Nothing;
            }

            // This always succeeds
            var k = ExtraParsers.CutOut(tokenizer);
            var m = k.TryParse(unfilteredSeriesName.Value);

            return ExtraParsers.CutOutOrAllInput(tokenizer).TryParse(unfilteredSeriesName.Value).Value.ToMaybe();
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
            get { return SeriesOpeningOrEndingParser; }
        }
        #endregion
    }
}
