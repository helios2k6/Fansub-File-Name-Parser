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
using System;

namespace FansubFileNameParser.Entity.Parsers
{
    /// <summary>
    /// The Original Animation parsers
    /// </summary>
    internal static class OriginalAnimationEntityParsers
    {
        #region private fields
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
        private static readonly Parser<string> SeriesName = ExtraParsers.CollectExcept(OAToken);
        #endregion
        #region Title and Episode Number
        private static readonly Parser<string> OATitle = ExtraParsers.CollectExcept(ExtraParsers.Int);

        private static readonly Parser<Tuple<Maybe<string>, Maybe<int>>> TitleThenEpisodeNumber =
            from title in OATitle.Token().OptionalMaybe()
            from episodeNumber in ExtraParsers.Int.OptionalMaybe()
            where title.HasValue || episodeNumber.HasValue
            select Tuple.Create(title.Select(t => t.Trim()), episodeNumber);

        private static readonly Parser<Tuple<Maybe<string>, Maybe<int>>> EpisodeNumberThenTitle =
            from episodeNumber in ExtraParsers.Int.Token().OptionalMaybe()
            from title in OATitle.Token().OptionalMaybe()
            where episodeNumber.HasValue || title.HasValue
            select Tuple.Create(title.Select(t => t.Trim()), episodeNumber);

        private static readonly Parser<Tuple<Maybe<string>, Maybe<int>>> TitleAndEpisodeNumber =
            TitleThenEpisodeNumber.Or(EpisodeNumberThenTitle);
        #endregion
        #region Composite Parsers
        private static readonly Parser<IFansubEntity> OriginalAnimationParser =
            from metadata in BaseEntityParsers.MediaMetadata.OptionalMaybe().ResetInput()
            from fansubGroup in BaseEntityParsers.FansubGroup.OptionalMaybe().ResetInput()
            from extension in FileEntityParsers.FileExtension.OptionalMaybe().ResetInput()
            from _1 in ExtraParsers.Filter(BaseGrammars.DashSeparatorToken).SetResultAsRemainder()
            from _2 in BaseGrammars.ContentBetweenTagGroups.SetResultAsRemainder()
            from series in SeriesName.OptionalMaybe()
            from oaToken in OAToken.Token()
            from titleAndEpisode in TitleAndEpisodeNumber.OptionalMaybe()
            let title = titleAndEpisode.HasValue ? titleAndEpisode.Value.Item1 : Maybe<string>.Nothing
            let episodeNumber = titleAndEpisode.HasValue ? titleAndEpisode.Value.Item2 : Maybe<int>.Nothing
            from _3 in ExtraParsers.RemainingCharacters
            select new FansubOriginalAnimationEntity
            {
                Group = fansubGroup,
                Series = series,
                Metadata = metadata,
                Extension = extension,
                Type = oaToken.ToMaybe(),
                Title = title,
                EpisodeNumber = episodeNumber,
            };
        #endregion
        #endregion
        #endregion
        #region public static properties
        /// <summary>
        /// Gets the original animation parser
        /// </summary>
        /// <value>
        /// The original animation parser
        /// </value>
        public static Parser<IFansubEntity> OriginalAnimation
        {
            get { return OriginalAnimationParser; }
        }
        #endregion
    }
}
