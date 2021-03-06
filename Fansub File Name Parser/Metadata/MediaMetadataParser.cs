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

using Functional.Maybe;
using Sprache;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FansubFileNameParser.Metadata
{
    /// <summary>
    /// Parses media metadata from the file name tags
    /// </summary>
    internal static class MediaMetadataParser
    {
        #region private fields
        private static readonly Regex CRC32Regex = new Regex(@"([A-F]|\d){8}");
        private static readonly Regex ResolutionRegEx = new Regex(@"(\D*)(\d{3,4})\s?x\s?(\d{3,4})(\D*)");
        #endregion

        #region public methods
        /// <summary>
        /// Tries the parse media metadata from the set of tags sent in
        /// </summary>
        /// <param name="tags">The tags.</param>
        /// <returns></returns>
        public static Maybe<MediaMetadata> TryParseMediaMetadata(IEnumerable<string> tags)
        {
            return TryParseImpl(tags);
        }
        #endregion

        #region private methods
        private static Maybe<MediaMetadata> TryParseImpl(IEnumerable<string> tags)
        {
            bool anythingTagged = false;
            var unusedTags = new List<string>();
            var metadata = new MediaMetadata();
            foreach (var tag in tags)
            {
                var anythingTaggedThisRound = false;

                //Match against all of the knowns
                AudioCodec outAudioCodec;
                if (TryGetAudioCodec(tag, out outAudioCodec))
                {
                    metadata.AudioCodec = outAudioCodec.ToMaybe();
                    anythingTagged = true;
                    anythingTaggedThisRound = true;
                }

                string outCrc32;
                if (TryGetCRC32Checksum(tag, out outCrc32))
                {
                    metadata.CRC32 = outCrc32.ToMaybe();
                    anythingTagged = true;
                    anythingTaggedThisRound = true;
                }

                PixelBitDepth outPixelBitDepth;
                if (TryGetPixelBitDepth(tag, out outPixelBitDepth))
                {
                    metadata.PixelBitDepth = outPixelBitDepth.ToMaybe();
                    anythingTagged = true;
                    anythingTaggedThisRound = true;
                }

                Resolution resolution;
                if (TryGetResolution(tag, out resolution))
                {
                    metadata.Resolution = resolution.ToMaybe();
                    anythingTagged = true;
                    anythingTaggedThisRound = true;
                }

                VideoCodec outVideoCodec;
                if (TryGetVideoCodec(tag, out outVideoCodec))
                {
                    metadata.VideoCodec = outVideoCodec.ToMaybe();
                    anythingTagged = true;
                    anythingTaggedThisRound = true;
                }

                VideoMedia outVideoMedia;
                if (TryGetVideoMedia(tag, out outVideoMedia))
                {
                    metadata.VideoMedia = outVideoMedia.ToMaybe();
                    anythingTagged = true;
                    anythingTaggedThisRound = true;
                }

                VideoMode outVideoMode;
                if (TryGetVideoMode(tag, out outVideoMode))
                {
                    metadata.VideoMode = outVideoMode.ToMaybe();
                    anythingTagged = true;
                    anythingTaggedThisRound = true;
                }

                if (anythingTaggedThisRound == false)
                {
                    unusedTags.Add(tag);
                }
            }

            metadata.UnusedTags = unusedTags;
            return anythingTagged ? metadata.ToMaybe() : Maybe<MediaMetadata>.Nothing;
        }

        private static bool TryFilterTag<T>(string tag, IEnumerable<KeyValuePair<string, T>> candidateTags, out T outputResult)
        {
            outputResult = default(T);

            var splitString = tag.Split(' ');
            foreach (var split in splitString)
            {
                var splitTagWithBracketsRemoved = split.Replace("[", string.Empty).Replace("]", string.Empty);
                foreach (var tagCandidateKvp in candidateTags)
                {
                    var tagCandidate = tagCandidateKvp.Key;
                    if (DoesTagContainTagCandidate(splitTagWithBracketsRemoved, tagCandidate))
                    {
                        outputResult = tagCandidateKvp.Value;
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool DoesTagContainTagCandidate(string tag, string tagCandidate)
        {
            var tagParser = from _1 in ExtraParsers.NonLetterAndNonNumber.AsString().Or(ExtraParsers.BeginnningOfLine)
                            from mainTag in Parse.IgnoreCase(tagCandidate).Text()
                            from _2 in ExtraParsers.NonLetterAndNonNumber.AsString().Or(Parse.LineTerminator)
                            select mainTag;
            var scanner = ExtraParsers.ScanFor(tagParser);
            var m = scanner.TryParse(tag);
            return scanner.TryParse(tag).WasSuccessful;
        }

        private static bool TryGetAudioCodec(string tag, out AudioCodec audioCodec)
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

        private static bool TryGetPixelBitDepth(string tag, out PixelBitDepth pixelBitDepth)
        {
            return TryFilterTag(tag, Tags.PixelBitDepthTags, out pixelBitDepth);
        }

        private static bool TryGetResolution(string tag, out Resolution resolution)
        {
            resolution = default(Resolution);

            var matches = ResolutionRegEx.Match(tag);

            if (matches.Success)
            {
                if (matches.Groups.Count >= 4)
                {
                    var widthString = matches.Groups[2].Value;
                    var heightString = matches.Groups[3].Value;

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

        private static bool TryGetVideoCodec(string tag, out VideoCodec videoCodec)
        {
            return TryFilterTag(tag, Tags.VideoCodecTags, out videoCodec);
        }

        private static bool TryGetVideoMedia(string tag, out VideoMedia videoMedia)
        {
            return TryFilterTag(tag, Tags.VideoMediaTags, out videoMedia);
        }

        private static bool TryGetVideoMode(string tag, out VideoMode videoMode)
        {
            return TryFilterTag(tag, Tags.VideoModeTags, out videoMode);
        }
        #endregion
    }
}
