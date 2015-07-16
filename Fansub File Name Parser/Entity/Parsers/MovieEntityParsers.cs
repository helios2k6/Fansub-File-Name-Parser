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
using Sprache;
using System;
using System.Collections.Generic;

namespace FansubFileNameParser.Entity.Parsers
{
    internal static class MovieEntityParsers
    {
        #region private static fields
        private static readonly Parser<int> NumberSignPrefixedMovieNumberToken =
            from _ in Parse.Char('#')
            from __ in Parse.WhiteSpace.Many()
            from movieNumber in ExtraParsers.Int
            select movieNumber;

        private static readonly Parser<int> EpisodePrefixedMovieNumberToken =
            from _ in Parse.IgnoreCase("EP")
            from __ in Parse.WhiteSpace.Many()
            from movieNumber in ExtraParsers.Int
            select movieNumber;

        private static readonly Parser<char> RomanNumeralToken = Parse.IgnoreCase('I').Or(Parse.IgnoreCase('V')).Or(Parse.IgnoreCase('X'));

        private static readonly Parser<int> RomanNumeralMovieNumber =
            from romanNumerals in RomanNumeralToken.AtLeastOnce().Text()
            select RomanNumerals.TranslateRomanNumeralToInt(romanNumerals);

        private static readonly Parser<int> MovieNumberParser =
            ExtraParsers.ScanFor(NumberSignPrefixedMovieNumberToken)
                .Or(ExtraParsers.ScanFor(EpisodePrefixedMovieNumberToken))
                .Or(ExtraParsers.ScanFor(RomanNumeralMovieNumber));

        private static readonly Parser<IFansubEntity> MovieParser =
            from metadata in BaseEntityParsers.MediaMetadata.OptionalMaybe().ResetInput()
            from fansubGroup in BaseEntityParsers.FansubGroup.OptionalMaybe().ResetInput()
            from series in BaseEntityParsers.SeriesName.OptionalMaybe().ResetInput()
            from extension in FileEntityParsers.FileExtension.OptionalMaybe().ResetInput()
            from movieNumber in MovieNumberParser.OptionalMaybe()
            select new FansubMovieEntity
            {
                Metadata = metadata,
                Group = fansubGroup,
                Series = series,
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
