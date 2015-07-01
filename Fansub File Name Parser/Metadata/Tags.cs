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
        public static IEnumerable<KeyValuePair<string, AudioCodec>> AudioTags
        {
            get
            {
                return new Dictionary<string, AudioCodec>
                {
                    {MediaMetadataTags.AAC, AudioCodec.AAC},
                    {MediaMetadataTags.AC3, AudioCodec.AC3},
                    {MediaMetadataTags.DTS, AudioCodec.DTS},
                    {MediaMetadataTags.FLAC, AudioCodec.FLAC},
                    {MediaMetadataTags.MP3, AudioCodec.MP3},
                    {MediaMetadataTags.OGG, AudioCodec.OGG},
                };
            }
        }

        /// <summary>
        /// Gets the pixel bit depth tags.
        /// </summary>
        /// <value>
        /// The pixel bit depth tags.
        /// </value>
        public static IEnumerable<KeyValuePair<string, PixelBitDepth>> PixelBitDepthTags
        {
            get
            {
                return new Dictionary<string, PixelBitDepth>
                {
                    {MediaMetadataTags.EightBit, PixelBitDepth.EightBits},
                    {MediaMetadataTags.EightBitWithSpace, PixelBitDepth.EightBits},
                    {MediaMetadataTags.TenBit, PixelBitDepth.TenBits},
                    {MediaMetadataTags.TenBitWithSpace, PixelBitDepth.TenBits},
                    {MediaMetadataTags.Hi10P, PixelBitDepth.TenBits},
                };
            }
        }

        /// <summary>
        /// Gets the video codec tags.
        /// </summary>
        /// <value>
        /// The video codec tags.
        /// </value>
        public static IEnumerable<KeyValuePair<string, VideoCodec>> VideoCodecTags
        {
            get
            {
                return new Dictionary<string, VideoCodec>
                {
                    {MediaMetadataTags.H264, VideoCodec.H264},
                    {MediaMetadataTags.X264, VideoCodec.H264},
                    {MediaMetadataTags.VC1, VideoCodec.VC1},
                    {MediaMetadataTags.XVID, VideoCodec.XVID},
                };
            }
        }

        /// <summary>
        /// Gets the video media tags.
        /// </summary>
        /// <value>
        /// The video media tags.
        /// </value>
        public static IEnumerable<KeyValuePair<string, VideoMedia>> VideoMediaTags
        {
            get
            {
                return new Dictionary<string, VideoMedia>
                {
                    {MediaMetadataTags.BD, VideoMedia.Bluray},
                    {MediaMetadataTags.BDRIP, VideoMedia.Bluray},
                    {MediaMetadataTags.BLURAY, VideoMedia.Bluray},
                    {MediaMetadataTags.BLURAY_WITH_DASH, VideoMedia.Bluray},
                    {MediaMetadataTags.DVD, VideoMedia.DVD},
                    {MediaMetadataTags.DVDRIP, VideoMedia.DVD},
                    {MediaMetadataTags.TV, VideoMedia.Broadcast},
                };
            }
        }

        /// <summary>
        /// Gets the video mode tags.
        /// </summary>
        /// <value>
        /// The video mode tags.
        /// </value>
        public static IEnumerable<KeyValuePair<string, VideoMode>> VideoModeTags
        {
            get
            {
                return new Dictionary<string, VideoMode>
                {
                    {MediaMetadataTags.FourEightyI, VideoMode.FourEightyInterlaced},
                    {MediaMetadataTags.FourEightIWithSpace, VideoMode.FourEightyInterlaced},
                    {MediaMetadataTags.FourEightyP, VideoMode.FourEightyProgressive},
                    {MediaMetadataTags.SevenTwentyP, VideoMode.SevenTwentyProgressive},
                    {MediaMetadataTags.TenEightyP, VideoMode.TenEightyProgressive},
                    {MediaMetadataTags.TenEightyI, VideoMode.TenEightyInterlaced},
                    {MediaMetadataTags.TenEightyIWithSpace, VideoMode.TenEightyInterlaced},
                    {MediaMetadataTags.FiveSeventySixProgressive, VideoMode.FiveSeventySixProgressive},
                    {MediaMetadataTags.FiveSeventySixProgressiveWithSpace, VideoMode.FiveSeventySixProgressive},
                    {MediaMetadataTags.FiveSeventySixInterlaced, VideoMode.FiveSeventySixInterlaced},
                    {MediaMetadataTags.FiveSeventySixInterlacedWithSpace, VideoMode.FiveSeventySixInterlaced},
                };
            }
        }
    }
}
