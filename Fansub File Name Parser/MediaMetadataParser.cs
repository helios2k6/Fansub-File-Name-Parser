using Sprache;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FansubFileNameParser
{
    public static class MediaMetadataParser
    {
        #region private fields
        private static readonly ISet<string> AudioTags = new HashSet<string>
        {
            MediaMetadataTags.AAC,
            MediaMetadataTags.DTS,
            MediaMetadataTags.FLAC,
            MediaMetadataTags.OGG
        };

        private static readonly ISet<string> PixelBitDepthTags = new HashSet<string>
        {

        };

        private static readonly ISet<string> ResolutionTags = new HashSet<string>
        {

        };

        private static readonly ISet<string> VideoCodecTags = new HashSet<string>
        {

        };

        private static readonly ISet<string> VideoMediaTags = new HashSet<string>
        {

        };

        private static readonly ISet<string> VideoModeTags = new HashSet<string>
        {

        };

        private static readonly Regex CRC32Regex = new Regex(@"([A-F]|\d){8}");
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
                if(TryGetAudioCodec(tag, out outputTag))
                {
                    metadata.AudioCodec = outputTag;
                }
                
                if(TryGetCRC32Checksum(tag, out outputTag))
                {
                    metadata.CRC32 = outputTag;
                }
                
                if(TryGetPixelBitDepth(tag, out outputTag))
                {
                    metadata.PixelBitDepth = outputTag;
                }
                
                if(TryGetResolution(tag, out outputTag))
                {
                    metadata.Resolution = outputTag;
                }
                
                if(TryGetVideoCodec(tag, out outputTag))
                {
                    metadata.VideoCodec = outputTag;
                }
                
                if(TryGetVideoMedia(tag, out outputTag))
                {
                    metadata.VideoMedia = outputTag;
                }
                
                if(TryGetVideoMode(tag, out outputTag))
                {
                    metadata.VideoMode = outputTag;
                }
            }

            return true;
        }
        #endregion

        #region private methods
        private static bool TryFilterTag(string tag, ISet<string> tagMatches, out string outputResult)
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
            return TryFilterTag(tag, AudioTags, out audioCodec);
        }

        private static bool TryGetCRC32Checksum(string tag, out string crc32Checksum)
        {
            crc32Checksum = string.Empty;

            var upperCased = tag.ToUpperInvariant();
            var matches = CRC32Regex.Match(tag);

            if(matches.Success)
            {
                crc32Checksum = matches.Groups[0].Value;
                return true;
            }

            return false;
        }

        private static bool TryGetPixelBitDepth(string tag, out string pixelBitDepth)
        {
            return TryFilterTag(tag, PixelBitDepthTags, out pixelBitDepth);
        }

        private static bool TryGetResolution(string tag, out string resolution)
        {
            return TryFilterTag(tag, ResolutionTags, out resolution);
        }

        private static bool TryGetVideoCodec(string tag, out string videoCodec)
        {
            return TryFilterTag(tag, VideoCodecTags, out videoCodec);
        }

        private static bool TryGetVideoMedia(string tag, out string videoMedia)
        {
            return TryFilterTag(tag, VideoMediaTags, out videoMedia);
        }

        private static bool TryGetVideoMode(string tag, out string videoMode)
        {
            return TryFilterTag(tag, VideoModeTags, out videoMode);
        }
        #endregion
    }
}
