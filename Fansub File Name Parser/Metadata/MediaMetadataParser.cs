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


using Sprache;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FansubFileNameParser.Metadata
{
    /// <summary>
    /// Parses media metadata from the file name tags
    /// </summary>
    public static class MediaMetadataParser
    {
        #region private fields
        private static readonly Regex CRC32Regex = new Regex(@"([A-F]|\d){8}");
        private static readonly Regex ResolutionRegEx = new Regex(@"(\d{3,4})\s?x\s?(\d{3,4})");
        #endregion

        #region public methods
        public static bool TryParseMediaMetadata(string fileName, out MediaMetadata metadata)
        {
            metadata = default(MediaMetadata);

            var parseResult = BaseGrammars.GrindTagsWithBracketsOutOfMajorContent.TryParse(fileName);
            if (parseResult.WasSuccessful == false)
            {
                return false;
            }

            var tags = parseResult.Value;

            metadata = new MediaMetadata();

            foreach (var tag in tags)
            {
                //Match against all of the knowns
                string outputTag;
                Resolution resolution;
                if (TryGetAudioCodec(tag, out outputTag))
                {
                    metadata.AudioCodec = outputTag;
                }

                if (TryGetCRC32Checksum(tag, out outputTag))
                {
                    metadata.CRC32 = outputTag;
                }

                if (TryGetPixelBitDepth(tag, out outputTag))
                {
                    metadata.PixelBitDepth = outputTag;
                }

                if (TryGetResolution(tag, out resolution))
                {
                    metadata.Resolution = resolution;
                }

                if (TryGetVideoCodec(tag, out outputTag))
                {
                    metadata.VideoCodec = outputTag;
                }

                if (TryGetVideoMedia(tag, out outputTag))
                {
                    metadata.VideoMedia = outputTag;
                }

                if (TryGetVideoMode(tag, out outputTag))
                {
                    metadata.VideoMode = outputTag;
                }
            }

            return true;
        }
        #endregion

        #region private methods
        private static bool TryFilterTag(string tag, IEnumerable<string> tagMatches, out string outputResult)
        {
            outputResult = default(string);

            var upperCased = tag.ToUpperInvariant();

            foreach (var tagCandidate in tagMatches)
            {
                if (upperCased.Contains(tagCandidate))
                {
                    outputResult = tagCandidate;
                    return true;
                }
            }

            return false;
        }

        private static bool TryGetAudioCodec(string tag, out string audioCodec)
        {
            return TryFilterTag(tag, Tags.AudioTags, out audioCodec);
        }

        private static bool TryGetCRC32Checksum(string tag, out string crc32Checksum)
        {
            crc32Checksum = string.Empty;

            var upperCased = tag.ToUpperInvariant();
            var matches = CRC32Regex.Match(upperCased);

            if (matches.Success)
            {
                crc32Checksum = matches.Groups[0].Value;
                return true;
            }

            return false;
        }

        private static bool TryGetPixelBitDepth(string tag, out string pixelBitDepth)
        {
            return TryFilterTag(tag, Tags.PixelBitDepthTags, out pixelBitDepth);
        }

        private static bool TryGetResolution(string tag, out Resolution resolution)
        {
            resolution = default(Resolution);

            var matches = ResolutionRegEx.Match(tag);

            if (matches.Success)
            {
                if (matches.Captures.Count >= 3)
                {
                    var widthString = matches.Groups[1].Value;
                    var heightString = matches.Groups[2].Value;

                    int width, height;
                    if (int.TryParse(widthString, out width) && int.TryParse(heightString, out height))
                    {
                        resolution = new Resolution(width, height);
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool TryGetVideoCodec(string tag, out string videoCodec)
        {
            return TryFilterTag(tag, Tags.VideoCodecTags, out videoCodec);
        }

        private static bool TryGetVideoMedia(string tag, out string videoMedia)
        {
            return TryFilterTag(tag, Tags.VideoMediaTags, out videoMedia);
        }

        private static bool TryGetVideoMode(string tag, out string videoMode)
        {
            return TryFilterTag(tag, Tags.VideoModeTags, out videoMode);
        }
        #endregion
    }
}
