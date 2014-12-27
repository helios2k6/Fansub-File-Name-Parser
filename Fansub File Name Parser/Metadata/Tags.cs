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

using System.Collections.Generic;

namespace FansubFileNameParser.Metadata
{
    /// <summary>
    /// A collection of media metadata tags
    /// </summary>
    public static class Tags
    {
        /// <summary>
        /// Gets the audio tags.
        /// </summary>
        /// <value>
        /// The audio tags.
        /// </value>
        public static IEnumerable<string> AudioTags
        {
            get
            {
                return new HashSet<string>
                {
                    MediaMetadataTags.AAC,
                    MediaMetadataTags.AC3,
                    MediaMetadataTags.DTS,
                    MediaMetadataTags.FLAC,
                    MediaMetadataTags.MP3,
                    MediaMetadataTags.OGG,
                };
            }
        }

        /// <summary>
        /// Gets the pixel bit depth tags.
        /// </summary>
        /// <value>
        /// The pixel bit depth tags.
        /// </value>
        public static IEnumerable<string> PixelBitDepthTags
        {
            get
            {
                return new HashSet<string>
                {
                    MediaMetadataTags.EightBit,
                    MediaMetadataTags.EightBitWithSpace,
                    MediaMetadataTags.TenBit,
                    MediaMetadataTags.TenBitWithSpace,
                    MediaMetadataTags.Hi10P,
                };
            }
        }

        /// <summary>
        /// Gets the video codec tags.
        /// </summary>
        /// <value>
        /// The video codec tags.
        /// </value>
        public static IEnumerable<string> VideoCodecTags
        {
            get
            {
                return new HashSet<string>
                {
                    MediaMetadataTags.H264,
                    MediaMetadataTags.X264,
                    MediaMetadataTags.XVID,
                };
            }
        }

        /// <summary>
        /// Gets the video media tags.
        /// </summary>
        /// <value>
        /// The video media tags.
        /// </value>
        public static IEnumerable<string> VideoMediaTags
        {
            get
            {
                return new HashSet<string>
                {
                    MediaMetadataTags.BD,
                    MediaMetadataTags.BDRIP,
                    MediaMetadataTags.BLURAY,
                    MediaMetadataTags.DVD,
                    MediaMetadataTags.DVDRIP,
                    MediaMetadataTags.TV,
                };
            }
        }

        /// <summary>
        /// Gets the video mode tags.
        /// </summary>
        /// <value>
        /// The video mode tags.
        /// </value>
        public static IEnumerable<string> VideoModeTags
        {
            get
            {
                return new HashSet<string>
                {
                    MediaMetadataTags.FourEightyP,
                    MediaMetadataTags.SevenTwentyP,
                    MediaMetadataTags.TenEightyP,
                };
            }
        }
    }
}
