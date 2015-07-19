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
using System.Globalization;
using System.Linq;

namespace FansubFileNameParser.Entity.Parsers
{
    /// <summary>
    /// The base parser for Fansub Entities
    /// </summary>
    internal static class BaseEntityParsers
    {
        #region private static fields
        private static Lazy<Parser<MediaMetadata>> MediaMetadataParserLazy =
            new Lazy<Parser<MediaMetadata>>(CreateMediaMetadataParser);

        private static Lazy<Parser<string>> FansubGroupParserLazy =
            new Lazy<Parser<string>>(CreateFansubGroupParser);

        private static Lazy<Parser<string>> SeriesNameParserLazy =
            new Lazy<Parser<string>>(CreateSeriesNameParser);

        private static readonly ISet<string> DateFormatStrings = new HashSet<string>
        {
            "yyyy",
        };
        #endregion
        #region private static methods
        private static Parser<MediaMetadata> CreateMediaMetadataParser()
        {
            Parser<MediaMetadata> parser = input =>
            {
                var substring = input.Source.Substring(input.Position);
                var tags = BaseGrammars.CollectTags.TryParse(substring);
                var mediaMetadata = tags.WasSuccessful
                    ? MediaMetadataParser.TryParseMediaMetadata(tags.Value)
                    : Maybe<MediaMetadata>.Nothing;

                if (mediaMetadata.IsSomething())
                {
                    return Result.Success<MediaMetadata>(mediaMetadata.Value, new Input(string.Empty));
                }

                return Result.Failure<MediaMetadata>(input, "Could not parse media metadata", new string[0]);
            };

            return parser.Memoize();
        }

        private static Parser<string> CreateFansubGroupParser()
        {
            DateTime _;
            Parser<string> group = from metadataVar in MediaMetadata
                                   let unusedTags = metadataVar.UnusedTags
                                   let filteredOutDate = unusedTags.Where(s => IsDate(s) == false)
                                   let groupCandidate = filteredOutDate.FirstOrDefault()
                                   select groupCandidate;

            return group.Memoize();
        }

        private static bool IsDate(string dateTimeString)
        {
            DateTime _t;
            if (DateTime.TryParse(dateTimeString, out _t))
            {
                return true;
            }

            foreach (var formatString in DateFormatStrings)
            {
                if (DateTime.TryParseExact(dateTimeString, formatString, CultureInfo.InvariantCulture, DateTimeStyles.None, out _t))
                {
                    return true;
                }
            }

            return false;
        }

        private static Parser<string> CreateSeriesNameParser()
        {
            var parser = from _ in BaseGrammars.ContentBetweenTagGroups.SetResultAsRemainder()
                         from content in BaseGrammars.LineUpToLastDashSeparatorToken.Or(BaseGrammars.LineUpToEpisodeNumberToken)
                         select content.Trim();

            return parser.Memoize();
        }
        #endregion
        #region public parsers
        /// <summary>
        /// Gets the MediaMetadata Parser
        /// </summary>
        /// <value>
        /// The MediaMetadata Parser
        /// </value>
        public static Parser<MediaMetadata> MediaMetadata
        {
            get { return MediaMetadataParserLazy.Value; }
        }

        /// <summary>
        /// Gets the Fansub Group parser
        /// </summary>
        /// <value>
        /// The Fansub Group parser
        /// </value>
        public static Parser<string> FansubGroup
        {
            get { return FansubGroupParserLazy.Value; }
        }

        /// <summary>
        /// Gets the Series Name parser
        /// </summary>
        /// <value>
        /// The Series Name parser
        /// </value>
        public static Parser<string> SeriesName
        {
            get { return SeriesNameParserLazy.Value; }
        }
        #endregion
    }
}
