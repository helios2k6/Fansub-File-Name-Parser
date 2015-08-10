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
using Sprache;
using System;

namespace FansubFileNameParser.Entity.Parsers
{
    /// <summary>
    /// Contains the Fansub Directory parser
    /// </summary>
    internal static class DirectoryEntityParsers
    {
        #region private fields
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

        private static readonly Parser<int> DashSeparatorTokenThenVolumeNumber =
            from _1 in BaseGrammars.DashSeparatorToken
            from _2 in VolumeNumber
            select _2;

        private static readonly Parser<Tuple<int, int>> DashSeparatorTokenThenEpisodeRange =
            from _1 in BaseGrammars.DashSeparatorToken
            from _2 in EpisodeRange
            select _2;

        private static readonly Parser<string> SeriesNameDirectory =
            from _1 in BaseGrammars.ContentBetweenTagGroups.SetResultAsRemainder()
            from seriesName in ExtraParsers.CollectExcept(
                ExtraParsers.Or(
                    EpisodeRange,
                    VolumeNumber,
                    DashSeparatorTokenThenEpisodeRange,
                    DashSeparatorTokenThenVolumeNumber
                )
            )
            select seriesName.Trim();

        private static readonly Parser<IFansubEntity> DirectoryParser =
            from _1 in ExtraParsers.ScanFor(BaseGrammars.FileExtension).Not().ResetInput()
            from metadata in BaseEntityParsers.MediaMetadata.OptionalMaybe().ResetInput()
            from fansubGroup in BaseEntityParsers.FansubGroup.OptionalMaybe().ResetInput()
            from series in SeriesNameDirectory.OptionalMaybe().ResetInput()
            from vol in ExtraParsers.ScanFor(VolumeNumber).OptionalMaybe()
            from range in ExtraParsers.ScanFor(EpisodeRange).OptionalMaybe()
            from _2 in ExtraParsers.RemainingCharacters
            select new FansubDirectoryEntity
            {
                Group = fansubGroup,
                Series = series,
                Metadata = metadata,
                Volume = vol,
                EpisodeRange = range,
            };
        #endregion
        #region public static properties
        /// <summary>
        /// Gets the directory parser
        /// </summary>
        /// <value>
        /// The directory parser
        /// </value>
        public static Parser<IFansubEntity> Directory
        {
            get { return DirectoryParser; }
        }
        #endregion
    }
}
