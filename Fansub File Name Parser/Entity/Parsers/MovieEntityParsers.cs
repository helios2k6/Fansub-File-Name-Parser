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

using FansubFileNameParser.Utils;
using Functional.Maybe;
using Sprache;
using System;
using System.Collections.Generic;

namespace FansubFileNameParser.Entity.Parsers
{
    internal static class MovieEntityParsers
    {
        #region private static fields
        private static readonly Parser<int> NumberSignPrefixedMovieNumberToken =
            from _1 in Parse.Char('#')
            from _2 in Parse.WhiteSpace.Many()
            from movieNumber in ExtraParsers.Int
            select movieNumber;

        private static readonly Parser<int> EpisodePrefixedMovieNumberToken =
            from _1 in Parse.IgnoreCase("EP")
            from _2 in Parse.WhiteSpace.Many()
            from movieNumber in ExtraParsers.Int
            select movieNumber;

        private static readonly Parser<char> RomanNumeralToken =
            Parse.IgnoreCase('I').Or(Parse.IgnoreCase('V')).Or(Parse.IgnoreCase('X'));

        private static readonly Parser<int> RomanNumeralMovieNumber =
            from romanNumerals in RomanNumeralToken.AtLeastOnce().Text()
            select RomanNumerals.TranslateRomanNumeralToInt(romanNumerals);

        private static readonly Parser<int> MovieNumberToken =
            from _1 in Parse.WhiteSpace
            from number in NumberSignPrefixedMovieNumberToken.Or(EpisodePrefixedMovieNumberToken).Or(RomanNumeralMovieNumber)
            select number;

        private static readonly Parser<IFansubEntity> MovieParser =
            from metadata in BaseEntityParsers.MediaMetadata.OptionalMaybe().ResetInput()
            from fansubGroup in BaseEntityParsers.FansubGroup.OptionalMaybe().ResetInput()
            from extension in FileEntityParsers.FileExtension.OptionalMaybe().ResetInput()
            from _1 in BaseGrammars.MainContent.SetResultAsRemainder()
            from seriesName in ExtraParsers.LineUpTo(MovieNumberToken).Intercept("Movie - Series Name").OptionalMaybe()
            from movieNumber in MovieNumberToken.OptionalMaybe()
            from _2 in BaseGrammars.DashSeparatorToken.OptionalMaybe()
            from subtitle in ExtraParsers.RemainingCharacters.AtLeastOneCharTrimmed().OptionalMaybe()
            select new FansubMovieEntity
            {
                Metadata = metadata,
                Group = fansubGroup,
                Series = seriesName.Select(t => t.Trim()),
                Subtitle = subtitle.Select(t => t.Trim()),
                Extension = extension,
                MovieNumber = movieNumber,
            };
        #endregion
        #region public properties
        /// <summary>
        /// Gets the Movie parser
        /// </summary>
        /// <value>
        /// The movie parser
        /// </value>
        public static Parser<IFansubEntity> Movie
        {
            get { return MovieParser; }
        }
        #endregion
    }
}
