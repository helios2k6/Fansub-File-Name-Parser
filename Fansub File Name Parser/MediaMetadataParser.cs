using Sprache;
using System.Collections.Generic;

namespace FansubFileNameParser
{
    public static class MediaMetadataParser
    {
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

            }

            return true;
        }

        private static bool TryGetAudioCodec(string tag, out string audioCodec)
        {
            audioCodec = string.Empty;

            var upperCased = tag.ToUpperInvariant();

            if (upperCased.Contains(MediaMetadataTags.AAC))
            {
                audioCodec = MediaMetadataTags.AAC;
                return true;
            }

            if (upperCased.Contains(MediaMetadataTags.FLAC))
            {
                audioCodec = MediaMetadataTags.FLAC;
                return true;
            }

            if (upperCased.Contains(MediaMetadataTags.OGG))
            {
                audioCodec = MediaMetadataTags.OGG;
                return true;
            }


            if (upperCased.Contains(MediaMetadataTags.DTS))
            {
                audioCodec = MediaMetadataTags.DTS;
                return true;
            }
            return false;
        }

        private static bool TryGetCRC32Checksum(string tag, out string crc32Checksum)
        {
            crc32Checksum = string.Empty;

            return false;
        }

        private static bool TryGetPixelBitDepth(string tag, out string pixelBitDepth)
        {
            pixelBitDepth = string.Empty;

            return false;
        }

        private static bool TryGetResolution(string tag, out string resolution)
        {
            resolution = string.Empty;

            return false;
        }

        private static bool TryGetVideoCodec(string tag, out string videoCodec)
        {
            videoCodec = string.Empty;
            return false;
        }

        private static bool TryGetVideoMedia(string tag, out string videoMedia)
        {
            videoMedia = string.Empty;
            return false;
        }

        private static bool TryGetVideoMode(string tag, out string videoMode)
        {
            videoMode = string.Empty;
            return false;
        }
    }
}
