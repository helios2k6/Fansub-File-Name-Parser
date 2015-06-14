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

using FansubFileNameParser.Entity.Directory;
using FansubFileNameParser.Metadata;
using Functional.Maybe;
using Sprache;
using System;
using System.Linq;

namespace FansubFileNameParser.Entity
{
    /// <summary>
    /// A static class of Parser{string} objects that parse entity specific strings
    /// </summary>
    internal static class EntityParsers
    {
        #region private nested class
        private class FileBasedParseResult : BaseParseResult
        {
            public Maybe<string> Extension { get; set; }
        }
        /// <summary>
        /// The result of an OP/ED Parser
        /// </summary>
        private sealed class OPEDParseResult
        {
            public Maybe<string> CreditlessPrefix { get; set; }
            public Maybe<string> OPEDToken { get; set; }
            public Maybe<int> SequenceNumber { get; set; }
        }

        /// <summary>
        /// The result of the base parser
        /// </summary>
        private class BaseParseResult
        {
            public Maybe<string> Group { get; set; }
            public Maybe<string> Series { get; set; }
            public Maybe<MediaMetadata> Metadata { get; set; }
        }
        #endregion
        #region private static fields
        private static readonly Lazy<Parser<BaseParseResult>> BaseEntityParserLazy =
            new Lazy<Parser<BaseParseResult>>(CreateBaseEntityParser);
        #endregion
        #region public static methods
        /// <summary>
        /// Tries to parse the string into an <see cref="IFansubEntity"/> object
        /// </summary>
        /// <param name="unprocessedString">The unprocessed string.</param>
        /// <returns>The parse result</returns>
        public static Maybe<IFansubEntity> TryParseEntity(string unprocessedString)
        {
            return Maybe<IFansubEntity>.Nothing;
        }
        #endregion
        #region private static methods
        #region Base
        /// <summary>
        /// Parses the data required to construct a <see cref="FansubEntityBase"/>. This parser will not consume
        /// any characters, but is subject to the remainder of any strings that were consumed, so it should always
        /// be the first parser in any parser comprehension.
        /// </summary>
        private static Parser<BaseParseResult> BaseEntity
        {
            get { return BaseEntityParserLazy.Value; }
        }

        private static Parser<BaseParseResult> CreateBaseEntityParser()
        {
            Parser<BaseParseResult> parser = input =>
            {
                var substring = input.Source.Substring(input.Column);
                var tags = BaseGrammars.CollectTags.TryParse(substring);
                var metadata = tags.WasSuccessful
                    ? MediaMetadataParser.TryParseMediaMetadata(tags.Value)
                    : Maybe<MediaMetadata>.Nothing;

                DateTime _;
                Maybe<string> group = from metadataVar in metadata
                                      let unusedTags = metadataVar.UnusedTags
                                      let filteredOutDate = unusedTags.Where(s => DateTime.TryParse(s, out _) == false)
                                      let groupCandidate = filteredOutDate.FirstOrDefault()
                                      select groupCandidate;

                Maybe<string> seriesName = Maybe<string>.Nothing;

                var mainContentParser = BaseGrammars.ContentBetweenTagGroups.ContinueWith(BaseGrammars.LineUpToDashSeparatorToken);
                var mainContent = mainContentParser.TryParse(substring);
                if (mainContent.WasSuccessful)
                {
                    var name = BaseGrammars.LineUpToDashSeparatorToken.TryParse(mainContent.Value);
                    seriesName = name.WasSuccessful
                        ? name.Value.Trim().ToMaybe()
                        : Maybe<string>.Nothing;
                }

                return Result.Success<BaseParseResult>(
                    new BaseParseResult
                    {
                        Group = group,
                        Series = seriesName,
                        Metadata = metadata,
                    },
                    input
                );
            };

            return parser.Memoize();
        }

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

        private static readonly Parser<int> VolumeNumber =
            from volumeToken in VolumeToken
            from dotAndSpace in Parse.AnyChar.Except(ExtraParsers.Int).Many()
            from number in ExtraParsers.Int
            select number;

        private static readonly Parser<IFansubEntity> Directory =
            from @base in BaseEntity
            from vol in ExtraParsers.ScanFor(VolumeNumber).OptionalMaybe()
            from range in ExtraParsers.ScanFor(EpisodeRange).OptionalMaybe()
            select new FansubDirectoryEntity
            {
                Group = @base.Group,
                Series = @base.Series,
                Metadata = @base.Metadata,
                Volume = vol,
                EpisodeRange = range,
            };
        #endregion
        #region File
        private static readonly Parser<FileBasedParseResult> FileEntityBase =
            (from @base in BaseEntity
             from ext in ExtraParsers.ScanFor(BaseGrammars.FileExtension)
             select new FileBasedParseResult
             {
                 Extension = ext.ToMaybe(),
                 Group = @base.Group,
                 Series = @base.Series,
                 Metadata = @base.Metadata,
             }).Memoize();
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
            from @base in FileEntityBase
            from openingToken in ExtraParsers.ScanFor(AnyOpeningToken)
            select new FansubOPEDEntity
            {
                Group = @base.Group,
                Series = @base.Series,
                Metadata = @base.Metadata,
                Extension = @base.Extension,
                SequenceNumber = openingToken.SequenceNumber,
                Part = FansubOPEDEntity.Segment.OP.ToMaybe(),
                NoCredits = openingToken.CreditlessPrefix.HasValue,
            };

        private static readonly Parser<IFansubEntity> ParseEnding =
            from @base in FileEntityBase
            from endingToken in ExtraParsers.ScanFor(AnyEndingToken)
            select new FansubOPEDEntity
            {
                Group = @base.Group,
                Series = @base.Series,
                Metadata = @base.Metadata,
                Extension = @base.Extension,
                SequenceNumber = endingToken.SequenceNumber,
                Part = FansubOPEDEntity.Segment.ED.ToMaybe(),
                NoCredits = endingToken.CreditlessPrefix.HasValue,
            };

        private static readonly Parser<IFansubEntity> OpeningOrEnding = ParseOpening.Or(ParseEnding);
        #endregion
        #endregion
        #region OVA / ONA / OAD
        #region Parsers
        private static readonly Parser<string> OVA = Parse.IgnoreCase("OVA").Text();

        private static readonly Parser<string> ONA = Parse.IgnoreCase("ONA").Text();

        private static readonly Parser<string> OAD = Parse.IgnoreCase("OAD").Text();

        private static readonly Parser<string> OAToken = OVA.Or(ONA).Or(OAD);

        private static readonly Parser<FansubOriginalAnimationEntity.ReleaseType> OVAReleaseType =
            from _ in OVA
            select FansubOriginalAnimationEntity.ReleaseType.OVA;

        private static readonly Parser<FansubOriginalAnimationEntity.ReleaseType> ONAReleaseType =
            from _ in ONA
            select FansubOriginalAnimationEntity.ReleaseType.ONA;

        private static readonly Parser<FansubOriginalAnimationEntity.ReleaseType> OADReleaseType =
            from _ in OAD
            select FansubOriginalAnimationEntity.ReleaseType.ONA;

        private static readonly Parser<FansubOriginalAnimationEntity.ReleaseType> ReleaseType =
            OVAReleaseType.Or(ONAReleaseType).Or(OADReleaseType).Memoize();

        private static readonly Parser<int> OAEpisodeNumberNoDash =
            from _ in OAToken
            from ep in ExtraParsers.Int
            select ep;

        private static readonly Parser<int> OAEpisodeNumberWithDash =
            from _ in OAToken
            from __ in BaseGrammars.DashSeparatorToken
            from ep in ExtraParsers.Int
            select ep;

        private static readonly Parser<int> OAEpisodeNumberToken =
            OAEpisodeNumberNoDash.Or(OAEpisodeNumberWithDash).Memoize();

        private static Parser<IFansubEntity> CreateOriginalAnimationParser()
        {
            return input =>
            {
                // Parse the basics
                var oaInfoParser = from @base in FileEntityBase.ResetInput()
                                   from type in ExtraParsers.ScanFor(ReleaseType).ResetInput()
                                   from episodeNumber in ExtraParsers.ScanFor(OAEpisodeNumberToken).Optional()
                                   select new { Base = @base, ReleaseType = type, EpisodeNumber = episodeNumber };

                var oaInfo = oaInfoParser.Invoke(input);
                if (oaInfo.WasSuccessful == false)
                {
                    return Result.Failure<IFansubEntity>(
                        oaInfo.Remainder,
                        string.Format(
                            "Could not parse the base file entity type or the Original Animation Release Type: {0}",
                            oaInfo.Message
                        ),
                        oaInfo.Expectations
                    );
                }

                // Do fancy stuff to parse the root series name and title of this OA



                // TODO DELETE THIS
                return Result.Failure<IFansubEntity>(
                    oaInfo.Remainder,
                    string.Format(
                        "Could not parse the base file entity type or the Original Animation Release Type: {0}",
                        oaInfo.Message
                    ),
                    oaInfo.Expectations
                );
            };
        }
        #endregion
        #endregion
        #endregion
    }
}
