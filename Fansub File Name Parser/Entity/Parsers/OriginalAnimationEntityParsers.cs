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

using FansubFileNameParser.Utils;
using Functional.Maybe;
using Sprache;
using System;
using System.Linq;

namespace FansubFileNameParser.Entity.Parsers
{
    /// <summary>
    /// The Original Animation parsers
    /// </summary>
    internal static class OriginalAnimationEntityParsers
    {
        #region private fields
        private static readonly Lazy<Parser<Tuple<Maybe<string>, Maybe<int>>>> TitleAndEpisodeParserLazy =
            new Lazy<Parser<Tuple<Maybe<string>, Maybe<int>>>>(CreateTitleAndEpisodeParser);

        #region OVA / ONA / OAD
        #region OA Tokens
        private static readonly Parser<FansubOriginalAnimationEntity.ReleaseType> OVA =
            Parse.IgnoreCase("OVA").Return(FansubOriginalAnimationEntity.ReleaseType.OVA);

        private static readonly Parser<FansubOriginalAnimationEntity.ReleaseType> ONA =
            Parse.IgnoreCase("ONA").Return(FansubOriginalAnimationEntity.ReleaseType.ONA);

        private static readonly Parser<FansubOriginalAnimationEntity.ReleaseType> OAD =
            Parse.IgnoreCase("OAD").Return(FansubOriginalAnimationEntity.ReleaseType.OAD);

        private static readonly Parser<FansubOriginalAnimationEntity.ReleaseType> OATypeToken =
            OVA.Or(ONA).Or(OAD);

        private static readonly Parser<FansubOriginalAnimationEntity.ReleaseType> DashThenOATypeToken =
            from _ in BaseGrammars.DashSeparatorToken
            from token in OATypeToken
            select token;

        private static readonly Parser<FansubOriginalAnimationEntity.ReleaseType> SpaceThenOATypeToken =
            from _1 in Parse.WhiteSpace
            from token in OATypeToken
            select token;

        private static readonly Parser<FansubOriginalAnimationEntity.ReleaseType> OAToken =
            DashThenOATypeToken.Or(SpaceThenOATypeToken);

        #endregion
        #region Root Name
        private static readonly Parser<string> SeriesName =
            ExtraParsers.LineUpTo(ExtraParsers.Or(OAToken, BaseGrammars.DashSeparatorToken));
        #endregion
        #region title and episode number
        private static Parser<Tuple<Maybe<string>, Maybe<int>>> TitleAndEpisodeParser
        {
            get { return TitleAndEpisodeParserLazy.Value; }
        }
        #endregion
        #region Composite Parsers
        private static readonly Parser<IFansubEntity> OriginalAnimationParser =
            from metadata in BaseEntityParsers.MediaMetadata.OptionalMaybe().ResetInput()
            from fansubGroup in BaseEntityParsers.FansubGroup.OptionalMaybe().ResetInput()
            from extension in FileEntityParsers.FileExtension.OptionalMaybe().ResetInput()
            from _1 in ExtraParsers.MainContent.SetResultAsRemainder()
            from series in SeriesName.OptionalMaybe()
            from oaToken in OAToken
            from titleAndEpisode in TitleAndEpisodeParser.OptionalMaybe()
            let title = titleAndEpisode.HasValue ? titleAndEpisode.Value.Item1 : Maybe<string>.Nothing
            let episodeNumber = titleAndEpisode.HasValue ? titleAndEpisode.Value.Item2 : Maybe<int>.Nothing
            from _4 in BaseGrammars.Line
            select new FansubOriginalAnimationEntity
            {
                Group = fansubGroup,
                Series = series.Select(s => s.Trim()),
                Metadata = metadata,
                Extension = extension,
                Type = oaToken.ToMaybe(),
                Title = title.Select(s => s.Trim()),
                EpisodeNumber = episodeNumber,
            };
        #endregion
        #endregion
        #endregion
        #region private static methods
        private static Parser<Tuple<Maybe<string>, Maybe<int>>> CreateTitleAndEpisodeParser()
        {
            // Uses a filtering style mechanism 
            Parser<Tuple<Maybe<string>, Maybe<int>>> parser = input =>
            {
                // Original input string
                var inputString = input.Source.Substring(input.Position);

                // Reference to final filtered string
                var finalFilteredString = inputString;

                // Remove any dash tokens
                var dashesFilteredResult = ExtraParsers.Filter(BaseGrammars.Dash).TryParse(finalFilteredString);
                if (dashesFilteredResult.WasSuccessful)
                {
                    finalFilteredString = dashesFilteredResult.Value;
                }

                // Trim
                finalFilteredString = finalFilteredString.Trim();

                // Search for the episode number
                var episodeNumberSearchResult = ExtraParsers.ScanFor(ExtraParsers.EpisodeWithVersionNumber).TryParse(finalFilteredString);
                var foundEpisodeNumber = Maybe<int>.Nothing;

                if (episodeNumberSearchResult.WasSuccessful)
                {
                    // Set the found episode number
                    foundEpisodeNumber = episodeNumberSearchResult.Value.Item1.ToMaybe();

                    // remove the episode number from the filtered string
                    var filteredEpisodeNumberStringResult =
                        ExtraParsers.Filter(ExtraParsers.EpisodeWithVersionNumber).TryParse(finalFilteredString);

                    // Only filter out the episode number if we found it
                    if (filteredEpisodeNumberStringResult.WasSuccessful)
                    {
                        finalFilteredString = filteredEpisodeNumberStringResult.Value;
                        finalFilteredString = finalFilteredString.Trim();
                    }
                }

                // Whatever is left must be the title
                var ovaTitle = string.IsNullOrWhiteSpace(finalFilteredString)
                    ? Maybe<string>.Nothing
                    : finalFilteredString.ToMaybe();

                // Check to see if we got the episode or the title
                if (foundEpisodeNumber.IsSomething() || ovaTitle.IsSomething())
                {
                    return Result.Success<Tuple<Maybe<string>, Maybe<int>>>(
                        Tuple.Create(ovaTitle, foundEpisodeNumber),
                        new Input(string.Empty)
                    );
                }

                // If we found neither, then send back failure
                return Result.Failure<Tuple<Maybe<string>, Maybe<int>>>(
                    input,
                    "Could not parse the episode number or title of the OVA",
                    Enumerable.Empty<string>()
                );
            };

            return parser;
        }
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
            get { return OriginalAnimationParser.Profile("OriginalAnimationParser"); }
        }
        #endregion
    }
}
