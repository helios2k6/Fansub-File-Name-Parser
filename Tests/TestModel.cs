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

using Functional.Maybe;
using FansubFileNameParser;
using FansubFileNameParser.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FansubFileNameParser.Entity;
using FansubFileNameParser.Entity.Directory;

namespace UnitTests.Models
{
    public static class TestModel
    {
        #region private fields
        private static readonly IDictionary<string, FansubFile> InputToFansubFileMap = new Dictionary<string, FansubFile>
        {
            {"[Aho-Taku] Sakurasou no Pet na Kanojo - 18 [720p-Hi10P][1D8F695D].mkv", new FansubFile("Aho-Taku", "Sakurasou no Pet na Kanojo", 18, ".mkv")},
            {"[Mazui]_Boku_Ha_Tomodachi_Ga_Sukunai_NEXT_-_05_[12F80420].mkv", new FansubFile("Mazui", "Boku Ha Tomodachi Ga Sukunai NEXT", 5, ".mkv")},
            {"[Anime-Koi] GJ-bu - 05 [h264-720p][E533CA00].mkv", new FansubFile("Anime-Koi", "GJ-bu", 5, ".mkv")},
            {"[WhyNot] Mayo Chiki - 10 [D1DA2637].mkv", new FansubFile("WhyNot", "Mayo Chiki", 10, ".mkv")},
            {"[HorribleSubs] Boku no Imouto wa Osaka Okan - 01 [720p].mkv", new FansubFile("HorribleSubs", "Boku no Imouto wa Osaka Okan", 1, ".mkv")},
            {"[Commie] Ore no Kanojo to Osananajimi ga Shuraba Sugiru - My Girlfriend and Childhood Friend Fight Too Much - 02 [F5ECCCC2].mkv",
                new FansubFile("Commie", "Ore no Kanojo to Osananajimi ga Shuraba Sugiru - My Girlfriend and Childhood Friend Fight Too Much", 2, ".mkv")},
            {"[Doki] Onii-chan Dakedo Ai Sae Areba Kankeinai yo ne - 01 (1280x720 Hi10P AAC) [B66EEF09].mkv", 
                new FansubFile("Doki", "Onii-chan Dakedo Ai Sae Areba Kankeinai yo ne", 1, ".mkv")},
            {"[FFF] Highschool DxD - SP01 [BD][1080p-FLAC][5D929653].mkv", new FansubFile("FFF", "Highschool DxD - SP01", int.MinValue, ".mkv")},
            {"[Eveyuu] Sankarea 00 [DVD Hi10P 480p H264] [4219AF02].mkv", new FansubFile("Eveyuu", "Sankarea", 0, ".mkv")},
            {"[gg]_Sasami-san@Ganbaranai_-_05_[6C2060E1].mkv", new FansubFile("gg", "Sasami-san@Ganbaranai", 5, ".mkv")},
            {"[RaX]Strawberry_Panic_-_01_[No_Dub]_(x264_ogg)_[F4EAA441].mkv", new FansubFile("RaX", "Strawberry Panic", 1, ".mkv")},
            {"(B-A)Devilman_Lady_-_01_(2E088B82).mkv", new FansubFile("B-A", "Devilman Lady", 1, ".mkv")},
            {"[Anime-Koi] GJ-bu - 06v2 [h264-720p][DAC4ACFA].mkv", new FansubFile("Anime-Koi", "GJ-bu", 6, ".mkv")},
            {"[Lunar] Bleach - 05 v2 [F2C9454F].avi", new FansubFile("Lunar", "Bleach", 5, ".avi")},
        };

        private static readonly Lazy<IEnumerable<KeyValuePair<string, MediaMetadata>>> InputToMediaMetadataMap =
            new Lazy<IEnumerable<KeyValuePair<string, MediaMetadata>>>(InitMediaMetadataTestModel);

        private static readonly IDictionary<string, IFansubEntity> DirectoryInputToEntityMap = new Dictionary<string, IFansubEntity>
        {
            {"Kokoro Connect (2012) [Doki-Chihiro][1920x1080 Hi10P BD FLAC]", new FansubDirectoryEntity 
                { 
                    Group = "Doki-Chihiro".ToMaybe(), 
                    Series = "Kokoro Connect".ToMaybe(),
                    MediaMetadata = new MediaMetadata
                    {
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                        PixelBitDepth = PixelBitDepth.TenBits.ToMaybe(),
                        Resolution = new Resolution(1920, 1080).ToMaybe(),
                        VideoCodec = VideoCodec.H264.ToMaybe(),
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                    }.ToMaybe(),
                }
            },
            {"[Vivid] The World God Only Knows - Goddesses Arc [BD 1080p FLAC]", new FansubDirectoryEntity
                {
                    Group = "Vivid".ToMaybe(),
                    Series = "The World God Only Knows - Goddesses Arc".ToMaybe(),
                    MediaMetadata = new MediaMetadata
                    {
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        VideoMode = VideoMode.TenEightyProgressive.ToMaybe(),
                    }.ToMaybe(),
                }
            },
            {"[BlurayDesuYo] Amagi Brilliant Park - Vol. 1 (BD 1920x1080 10bit FLAC)", new FansubDirectoryEntity
                {
                    Group = "BlurayDesuYo".ToMaybe(),
                    Series = "Amagi Brilliant Park".ToMaybe(),
                    Volume = 1.ToMaybe(),
                    MediaMetadata = new MediaMetadata
                    {
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        PixelBitDepth = PixelBitDepth.TenBits.ToMaybe(),
                        Resolution = new Resolution(1920, 1080).ToMaybe(),
                    }.ToMaybe(),
                }
            },
            {"[Coalgirls]_Amagi_Brilliant_Park_01-02_(1920x1080_Blu-Ray_FLAC)", new FansubDirectoryEntity
                {
                    Group = "Coalgirls".ToMaybe(),
                    Series = "Amagi Brilliant Park".ToMaybe(),
                    EpisodeRange = Tuple.Create(1, 2).ToMaybe(),
                    MediaMetadata = new MediaMetadata
                    {
                        Resolution = new Resolution(1920, 1080).ToMaybe(),
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                    }.ToMaybe(),
                }
            },
            {"[FFF] Medaka Box Abnormal - Vol.03 [BD][1080p-FLAC]", new FansubDirectoryEntity
                {
                    Group = "FFF".ToMaybe(),
                    Series = "Medaka Box Abnormal".ToMaybe(),
                    Volume = 3.ToMaybe(),
                    MediaMetadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        VideoMode = VideoMode.TenEightyProgressive.ToMaybe(),
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                    }.ToMaybe(),
                }
            },
            {"[Tsundere] Fate Kaleid Prisma Illya 2wei - 01-02 [BDRip h264 1920x1080 10bit FLAC]", new FansubDirectoryEntity
                {
                    Group = "Tsundere".ToMaybe(),
                    Series = "Fate Kaleid Prisma Illya 2wei".ToMaybe(),
                    EpisodeRange = Tuple.Create(1, 2).ToMaybe(),
                    MediaMetadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        VideoCodec = VideoCodec.H264.ToMaybe(),
                        Resolution = new Resolution(1920, 1080).ToMaybe(),
                        PixelBitDepth = PixelBitDepth.TenBits.ToMaybe(),
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                    }.ToMaybe(),
                }
            },
            {"[Elysium]Spice.and.Wolf.II(BD.1080p.FLAC)", new FansubDirectoryEntity
                {
                    Group = "Elysium".ToMaybe(),
                    Series = "Spiace and Wolf II".ToMaybe(),
                    MediaMetadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        VideoMode = VideoMode.TenEightyProgressive.ToMaybe(),
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                    }.ToMaybe(),
                }
            },
            {"[Yabai]_Spice_and_Wolf_II_[BD]", new FansubDirectoryEntity
                {
                    Group = "Yabai".ToMaybe(),
                    Series = "Spice and Wolf II".ToMaybe(),
                    MediaMetadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                    }.ToMaybe(),
                }
            },
            {"[Zurako] Spice and Wolf II (BD 1080p AAC)", new FansubDirectoryEntity
                {
                    Group = "Zurako".ToMaybe(),
                    Series = "Spice and Wolf II".ToMaybe(),
                    MediaMetadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        VideoMode = VideoMode.TenEightyProgressive.ToMaybe(),
                        AudioCodec = AudioCodec.AAC.ToMaybe(),
                    }.ToMaybe(),
                }
            },
            {"[UTW]_Fate_stay_night_Unlimited_Blade_Works_-_00-12_[BD][h264-1080p][FLAC]", new FansubDirectoryEntity
                {
                    Group = "UTW".ToMaybe(),
                    Series = "Fate stay night Unlimited Blade Works".ToMaybe(),
                    EpisodeRange = Tuple.Create(0, 12).ToMaybe(),
                    MediaMetadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        VideoCodec = VideoCodec.H264.ToMaybe(),
                        VideoMode = VideoMode.TenEightyProgressive.ToMaybe(),
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                    }.ToMaybe(),
                }
            },
            {"[Commie] Chihayafuru 2 - Volume 1 [BD 720p AAC]", new FansubDirectoryEntity
                {
                    Group = "Commie".ToMaybe(),
                    Series = "Chihayafuru 2".ToMaybe(),
                    Volume = 1.ToMaybe(),
                    MediaMetadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        VideoMode = VideoMode.SevenTwentyProgressive.ToMaybe(),
                        AudioCodec = AudioCodec.AAC.ToMaybe(),
                    }.ToMaybe(),
                }
            },
            {"[Coalgirls]_Hyouka_(1920x1080_Blu-Ray_FLAC)", new FansubDirectoryEntity
                {
                    Group = "Coalgirls".ToMaybe(),
                    Series = "Hyouka".ToMaybe(),
                    MediaMetadata = new MediaMetadata
                    {
                        Resolution = new Resolution(1920, 1080).ToMaybe(),
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                    }.ToMaybe(),
                }
            },
        };
        #endregion

        #region public methods
        /// <summary>
        /// Creates a new test model.
        /// </summary>
        /// <returns>A dictionary representing the test model</returns>
        public static IDictionary<string, FansubFile> CreateFansubFileTestModel()
        {
            return new Dictionary<string, FansubFile>(InputToFansubFileMap);
        }

        /// <summary>
        /// Creates the media metadata test model.
        /// </summary>
        /// <returns>An IEnumerable of key-value pairs of names to media metadata objects</returns>
        public static IEnumerable<KeyValuePair<string, MediaMetadata>> CreateMediaMetadataTestModel()
        {
            return InputToMediaMetadataMap.Value;
        }
        #endregion

        #region private methods
        private static IEnumerable<KeyValuePair<string, MediaMetadata>> CreateMapping(
            KeyValuePair<string, AudioCodec> audioTag,
            KeyValuePair<string, PixelBitDepth> pixelBitDepthTag,
            KeyValuePair<string, VideoCodec> videoCodecTag,
            KeyValuePair<string, VideoMedia> videoMediaTag,
            KeyValuePair<string, VideoMode> videoModeTag,
            KeyValuePair<string, Resolution> resolutionTag,
            string crc32
        )
        {
            var list = new List<string>
            {
                audioTag.Key,
                pixelBitDepthTag.Key,
                videoCodecTag.Key,
                videoMediaTag.Key,
                videoModeTag.Key,
                resolutionTag.Key,
                crc32,
            };

            var dummyFansubGroupTag = "[Dummy]";
            var dummyFansubAnimeName = "Dummy Fansub - 01";
            var dummyFileExtension = ".dummy";

            for (int i = 0; i < 127; i++)
            {
                var builder = new StringBuilder();
                var mediaMetadata = new MediaMetadata();

                builder.Append(dummyFansubGroupTag).Append(dummyFansubAnimeName);

                for (int j = 0; j < 7; j++)
                {
                    var token = list[j];

                    int bitAt = (1 << j) & i;
                    if (bitAt > 0)
                    {
                        builder.Append("[").Append(token).Append("]");

                        switch (j)
                        {
                            case 0:
                                mediaMetadata.AudioCodec = audioTag.Value.ToMaybe();
                                break;
                            case 1:
                                mediaMetadata.PixelBitDepth = pixelBitDepthTag.Value.ToMaybe();
                                break;
                            case 2:
                                mediaMetadata.VideoCodec = videoCodecTag.Value.ToMaybe();
                                break;
                            case 3:
                                mediaMetadata.VideoMedia = videoMediaTag.Value.ToMaybe();
                                break;
                            case 4:
                                mediaMetadata.VideoMode = videoModeTag.Value.ToMaybe();
                                break;
                            case 5:
                                mediaMetadata.Resolution = resolutionTag.Value.ToMaybe();
                                break;
                            case 6:
                                mediaMetadata.CRC32 = token.ToMaybe();
                                break;
                            default:
                                break;
                        }
                    }
                }

                builder.Append(dummyFileExtension);

                yield return new KeyValuePair<string, MediaMetadata>(builder.ToString(), mediaMetadata);
            }
        }

        private static IEnumerable<T> ConcatWithDynamicCheck<T>(this IEnumerable<T> @this, IEnumerable<T> other)
        {
            List<T> thisList = @this as List<T>;
            if (thisList == null)
            {
                thisList = new List<T>(@this);
            }

            thisList.AddRange(other);
            return thisList;
        }

        private static IEnumerable<KeyValuePair<string, MediaMetadata>> InitMediaMetadataTestModel()
        {
            IEnumerable<KeyValuePair<string, MediaMetadata>> kvps = Enumerable.Empty<KeyValuePair<string, MediaMetadata>>();

            foreach (var audioTag in Tags.AudioTags)
            {
                foreach (var pixelBitDepthTag in Tags.PixelBitDepthTags)
                {
                    foreach (var videoCodecTag in Tags.VideoCodecTags)
                    {
                        foreach (var videoMediaTag in Tags.VideoMediaTags)
                        {
                            foreach (var videoModeTag in Tags.VideoModeTags)
                            {
                                //Generate crc32
                                var crc32 = "01234567";

                                //Generate resolution
                                var res1 = "1920x1080";
                                var res2 = "1280x720";
                                var res3 = "720x480";
                                var res4 = "1920 x 1080";
                                var res5 = "1280 x 720";
                                var res6 = "720 x 480";

                                var map1 = CreateMapping(
                                    audioTag,
                                    pixelBitDepthTag,
                                    videoCodecTag,
                                    videoMediaTag,
                                    videoModeTag,
                                    new KeyValuePair<string, Resolution>(res1, new Resolution(1920, 1080)),
                                    crc32);

                                var map2 = CreateMapping(
                                    audioTag,
                                    pixelBitDepthTag,
                                    videoCodecTag,
                                    videoMediaTag,
                                    videoModeTag,
                                     new KeyValuePair<string, Resolution>(res2, new Resolution(1280, 720)),
                                    crc32);

                                var map3 = CreateMapping(
                                    audioTag,
                                    pixelBitDepthTag,
                                    videoCodecTag,
                                    videoMediaTag,
                                    videoModeTag,
                                    new KeyValuePair<string, Resolution>(res3, new Resolution(720, 480)),
                                    crc32);

                                var map4 = CreateMapping(
                                    audioTag,
                                    pixelBitDepthTag,
                                    videoCodecTag,
                                    videoMediaTag,
                                    videoModeTag,
                                    new KeyValuePair<string, Resolution>(res4, new Resolution(1920, 1080)),
                                    crc32);

                                var map5 = CreateMapping(
                                    audioTag,
                                    pixelBitDepthTag,
                                    videoCodecTag,
                                    videoMediaTag,
                                    videoModeTag,
                                    new KeyValuePair<string, Resolution>(res5, new Resolution(1280, 720)),
                                    crc32);

                                var map6 = CreateMapping(
                                    audioTag,
                                    pixelBitDepthTag,
                                    videoCodecTag,
                                    videoMediaTag,
                                    videoModeTag,
                                    new KeyValuePair<string, Resolution>(res6, new Resolution(720, 480)),
                                    crc32);

                                kvps = kvps
                                    .ConcatWithDynamicCheck(map1)
                                    .ConcatWithDynamicCheck(map2)
                                    .ConcatWithDynamicCheck(map3)
                                    .ConcatWithDynamicCheck(map4)
                                    .ConcatWithDynamicCheck(map5)
                                    .ConcatWithDynamicCheck(map6);
                            }
                        }
                    }
                }
            }

            return kvps;
        }
        #endregion
    }
}
