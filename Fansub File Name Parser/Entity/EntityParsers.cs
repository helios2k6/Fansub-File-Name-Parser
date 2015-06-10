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
            from _ in Parse.AnyChar.ExceptAny(EpisodeRange, VolumeToken).Many().Text().Optional()
            from vol in VolumeNumber.Optional()
            from range in EpisodeRange.Optional()
            select new FansubDirectoryEntity
            {
                Group = @base.Group,
                Series = @base.Series,
                Metadata = @base.Metadata,
                Volume = vol.ConvertFromIOptionToMaybe(),
                EpisodeRange = range.ConvertFromIOptionToMaybe(),
            };
        #endregion
        #region File
        private static readonly Parser<FileBasedParseResult> FileEntityBase =
            (from @base in BaseEntity
             from _ in Parse.AnyChar.Except(BaseGrammars.FileExtension).Many()
             from ext in BaseGrammars.FileExtension
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

        private static readonly Parser<IFansubEntity> ParseOpening =
            from @base in FileEntityBase
            from _ in Parse.AnyChar.Except(AnyOpeningToken).Many().Optional()
            from openingToken in AnyOpeningToken
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
            from _ in Parse.AnyChar.Except(AnyEndingToken).Many().Optional()
            from openingToken in AnyEndingToken
            select new FansubOPEDEntity
            {
                Group = @base.Group,
                Series = @base.Series,
                Metadata = @base.Metadata,
                Extension = @base.Extension,
                SequenceNumber = openingToken.SequenceNumber,
                Part = FansubOPEDEntity.Segment.ED.ToMaybe(),
                NoCredits = openingToken.CreditlessPrefix.HasValue,
            };

        private static readonly Parser<IFansubEntity> OpeningOrEnding = ParseOpening.Or(ParseEnding);
        #endregion
        #endregion
    }
}
