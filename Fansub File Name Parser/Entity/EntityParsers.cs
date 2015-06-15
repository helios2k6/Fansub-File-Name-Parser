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
            var parseResult = EntityParser.TryParse(unprocessedString);
            if (parseResult.WasSuccessful)
            {
                return parseResult.Value.ToMaybe();
            }

            return Maybe<IFansubEntity>.Nothing;
        }
        #endregion
        #region private static methods
        #region Top level
        private static readonly Parser<IFansubEntity> EntityParser =
            (from _ in BaseGrammars.CleanInputString.SetResultAsRemainder()
             from entity in Directory.Or(OpeningOrEnding).Or(OriginalAnimation)
             select entity).Memoize();
        #endregion
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

                var mainContentParser = BaseGrammars.ContentBetweenTagGroups.ContinueWith(BaseGrammars.LineUpToLastDashSeparatorToken);
                var mainContent = mainContentParser.TryParse(substring);
                if (mainContent.WasSuccessful)
                {
                    seriesName = mainContent.WasSuccessful
                        ? mainContent.Value.Trim().ToMaybe()
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
        #region OA Tokens
        private static readonly Parser<FansubOriginalAnimationEntity.ReleaseType> OVA = 
            Parse.IgnoreCase("OVA").Return(FansubOriginalAnimationEntity.ReleaseType.OVA);

        private static readonly Parser<FansubOriginalAnimationEntity.ReleaseType> ONA =
            Parse.IgnoreCase("ONA").Return(FansubOriginalAnimationEntity.ReleaseType.ONA);

        private static readonly Parser<FansubOriginalAnimationEntity.ReleaseType> OAD =
            Parse.IgnoreCase("OAD").Return(FansubOriginalAnimationEntity.ReleaseType.OAD);

        private static readonly Parser<FansubOriginalAnimationEntity.ReleaseType> OAToken =
            OVA.Or(ONA).Or(OAD).Memoize();
        #endregion
        #region Root Name
        private static readonly Parser<string> RootName = Parse.AnyChar.Except(OAToken).Many().Text().Token();
        #endregion
        #region Title and Episode Number
        private static readonly Parser<string> OATitle = Parse.AnyChar.Except(ExtraParsers.Int).Many().Text();

        private static readonly Parser<Tuple<Maybe<string>, Maybe<int>>> TitleThenEpisodeNumber =
            from title in OATitle.Token().OptionalMaybe()
            from episodeNumber in ExtraParsers.Int.OptionalMaybe()
            where title.HasValue || episodeNumber.HasValue
            select Tuple.Create(title, episodeNumber);

        private static readonly Parser<Tuple<Maybe<string>, Maybe<int>>> EpisodeNumberThenTitle =
            from episodeNumber in ExtraParsers.Int.Token().OptionalMaybe()
            from title in OATitle.Token().OptionalMaybe()
            where episodeNumber.HasValue || title.HasValue
            select Tuple.Create(title, episodeNumber);

        private static readonly Parser<Tuple<Maybe<string>, Maybe<int>>> TitleAndEpisodeNumber = 
            TitleThenEpisodeNumber.Or(EpisodeNumberThenTitle);
        #endregion
        #region Composite Parsers
        private static readonly Parser<IFansubEntity> OriginalAnimation =
            from @base in FileEntityBase
            from _ in ExtraParsers.Filter(BaseGrammars.DashSeparatorToken).SetResultAsRemainder()
            from rootName in BaseGrammars.ContentBetweenTagGroups.ContinueWith(RootName)
            from oaToken in OAToken.Token()
            from titleAndEpisode in TitleAndEpisodeNumber
            select new FansubOriginalAnimationEntity
            {
                Group = @base.Group,
                Series = rootName.ToMaybe(),
                Metadata = @base.Metadata,
                Extension = @base.Extension,
                Type = oaToken.ToMaybe(),
                Title = titleAndEpisode.Item1,
                EpisodeNumber = titleAndEpisode.Item2,
            };
        #endregion
        #endregion
        #region Episode

        #endregion
        #endregion
    }
}
