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

namespace FansubFileNameParser.Entity.Parsers
{
    /// <summary>
    /// Contains the Episode Entity parsers
    /// </summary>
    internal static class EpisodeEntityParsers
    {
        #region private static fields
        private static readonly Parser<IFansubEntity> EpisodeParser =
            from metadata in BaseEntityParsers.MediaMetadata.OptionalMaybe().ResetInput()
            from fansubGroup in BaseEntityParsers.FansubGroup.OptionalMaybe().ResetInput()
            from series in BaseEntityParsers.SeriesName.OptionalMaybe().ResetInput()
            from extension in FileEntityParsers.FileExtension.OptionalMaybe().ResetInput()
            from _1 in ExtraParsers.MainContent.SetResultAsRemainder()
            from episode in ExtraParsers.ScanFor(ExtraParsers.EpisodeVersionWithSpaceToken.Last())
            from remainingText in BaseGrammars.Line.AtLeastOneCharTrimmed().OptionalMaybe()
            where remainingText.HasValue == false
            select new FansubEpisodeEntity
            {
                Metadata = metadata,
                Group = fansubGroup,
                Series = series,
                Extension = extension,
                EpisodeNumber = episode.Item1.ToMaybe(),
            };
        #endregion

        #region public static properties
        /// <summary>
        /// Gets the Episode parser
        /// </summary>
        /// <value>
        /// The Episode parser
        /// </value>
        public static Parser<IFansubEntity> Episode
        {
            get { return EpisodeParser.Profile("EpisodeParser"); }
        }
        #endregion
    }
}
