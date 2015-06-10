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

using FansubFileNameParser.Entity;
using FansubFileNameParser.Entity.Directory;
using FansubFileNameParser.Metadata;
using Functional.Maybe;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests.Models
{
    public static class TestModel
    {
        #region private fields
        private static readonly Lazy<IEnumerable<KeyValuePair<IEnumerable<string>, MediaMetadata>>> InputToMediaMetadataMap =
            new Lazy<IEnumerable<KeyValuePair<IEnumerable<string>, MediaMetadata>>>(InitMediaMetadataTestModel);

        private static readonly IDictionary<string, IFansubEntity> OriginalVideoInputToEntityMap = new Dictionary<string, IFansubEntity>
        {
            {"[SFW] Mahou Sensei Negima! OAD ~Mou Hitotsu no Sekai~ 02 [DVD][87942D8A].mkv", null},
            {"[Hatsuyuki]_Noragami_OAD_-_01_[10bit][848x480][FDCA30AA].mkv", null},
            {"[SubDESU]_R-15_OAD_(DVD_720x480_x264_AAC)_[7D1F1EDF].mkv", null},
            {"[Chihiro]_Hitsugi_no_Chaika_-_OAD_[1280x720_Blu-ray_AAC][7ECAA5A1].mkv", null},
            {"[Saizen]_Attack_on_Titan_-_OAD_1_[06EA79B8].mkv", null},
            {"[gg]_Joshiraku_-_OAD_[869F6659].mkv", null},
            {"[Vivid] Tonari no Seki-kun - OAD [DVD 576p AAC][9D6909E7].mkv", null},
            {"[WCP] Nisekoi OAD 3 - Bath House & Service [576p][BCBBA0B2].mkv", null},
            {"[AniYoshi]_School_Days_OVA_-_Valentine_Days_(DVD)_[5DE6B483].mkv", null},
            {"[Coalgirls]_Valkyria_Chronicles_(1920x1080_Blu-Ray_FLAC)/[Coalgirls]_Valkyria_Chronicles_OVA_08_(1920x1080_Blu-Ray_FLAC)_[F015CD71].mkv", null},
            {"[Doki] Seto no Hanayome OVA - 01 (720x480 h264 DVD AAC) [032D22A4].mkv", null},
            {"[Procrastinating] Seitokai Yakuindomo OVA 03 [BD 1080p].mkv", null},
            {"[Ayako-Himatsubushi] Seitokai Yakuindomo OVA - 01v2 [DVD][480p][EF82D238].mkv", null},
            {"[Coalgirls]_Utawarerumono_OVA_03_(1920x1080_Blu-Ray_FLAC)_[D3BD62B6].mkv", null},
            {"The World God Only Knows OVA - Four Plus an Idol.mkv", null},
            {"[TastyMelon] Black Lagoon OVA - Roberta's Blood Trail - 04 [BD][480p][926257C1].mkv", null},
            {"[UTW]_Fate_Kaleid_Liner_Prisma_Ilya_-_OVA_[BD][h264-1080p_FLAC][002240CB].mkv", null},
        };

        private static readonly IDictionary<string, IFansubEntity> OpeningEndingInputToEntityMap = new Dictionary<string, IFansubEntity>
        {
            {"[FFF] Hyakka Ryouran Samurai Bride - ED06 [BD][1080p-FLAC][2218ADB6].mkv", null},
            {"[Ryuumaru] Hyakka Ryouran Samurai Girls - ED (Clean) [1080p - Bluray][DB8185D1].mkv", null},
            {"[FFF] Hataraku Maou-sama! - ED02 [BD][1080p-FLAC][577E003B].mkv", null},
            {"[Doki] GJ-bu - NCED 2 (1920x1080 Hi10P BD FLAC) [71E9D167].mkv", null},
            {"[Coalgirls]_Yahari_Ore_no_Seishun_Love_Comedy_wa_Machigatteiru_NCED3_(1920x1080_Blu-Ray_FLAC)_[759447B3].mkv", null},
            {"[Underwater-FFF] Saki Achiga-hen - Episode of Side-A - ED02 [BD][1080p-FLAC][280EFFB2].mkv", null},
            {"[Commie] Yuyushiki - NCED 09 [BD 720p AAC] [E291A643].mkv", null},
            {"[UTW]_Shinsekai_Yori_-_Creditless_ED2_[BD][h264-1080p_FLAC][E3C12E42].mkv", null},
            {"[Coalgirls]_C3-Cube_x_Cursed_x_Curious_NCED_(1920x1080_Blu-Ray_FLAC)_[50450AA5].mkv", null},
            {"[Coalgirls]_Valkyria_Chronicles_OP1_-_alt_(1920x1080_Blu-Ray_FLAC)_[414F6192].mkv", null},
            {"[Doki] UN-GO - NCOP (1920x1080 Hi10P BD FLAC) [8C5BF375].mkv", null},
            {"[Coalgirls]_Tokyo_Ghoul_NCOP1_(1920x1080_Blu-ray_FLAC)_[D91E552D].mkv", null},
            {"[WhyNot] Steins;Gate - NCOP [BD 720p AAC][1B65071D].mkv", null},
            {"[FFF] NouCome - OP02 [BD][1080p-FLAC][03E64080].mkv", null},
            {"[Final8]Mirai Nikki (Creditless OP3 - The Live World) (BD 10-bit 1280x720 x264 AAC)[8F8B757F].mkv", null},
            {"[Doki] Air - NCOP A (1280x720 Hi10P BD FLAC) [43076542].mkv", null},
            {"[joseole99] Gurren Lagann - NCOP3v2 (853x480 h264 AC3 AC3) [956587FD].mkv", null},
            {"[Doki] Yuru Yuri 2 - NCOP 2v2 (1920x1080 Hi10P BD FLAC) [8CABBF62].mkv", null},
        };

        private static readonly IDictionary<string, IFansubEntity> DirectoryInputToEntityMap = new Dictionary<string, IFansubEntity>
        {
            {"Kokoro Connect (2012) [Doki-Chihiro][1920x1080 Hi10P BD FLAC]", new FansubDirectoryEntity 
                { 
                    Group = "Doki-Chihiro".ToMaybe(), 
                    Series = "Kokoro Connect".ToMaybe(),
                    Metadata = new MediaMetadata
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
                    Metadata = new MediaMetadata
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
                    Metadata = new MediaMetadata
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
                    Metadata = new MediaMetadata
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
                    Metadata = new MediaMetadata
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
                    Metadata = new MediaMetadata
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
                    Metadata = new MediaMetadata
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
                    Metadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                    }.ToMaybe(),
                }
            },
            {"[Zurako] Spice and Wolf II (BD 1080p AAC)", new FansubDirectoryEntity
                {
                    Group = "Zurako".ToMaybe(),
                    Series = "Spice and Wolf II".ToMaybe(),
                    Metadata = new MediaMetadata
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
                    Metadata = new MediaMetadata
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
                    Metadata = new MediaMetadata
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
                    Metadata = new MediaMetadata
                    {
                        Resolution = new Resolution(1920, 1080).ToMaybe(),
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                    }.ToMaybe(),
                }
            },
            {"[Doki] Mahouka Koukou no Rettousei - Vol 1 (1920x1080 Hi10P BD FLAC)", new FansubDirectoryEntity
                {
                    Group = "Doki".ToMaybe(),
                    Series = "Mahouka Koukou no Rettousei".ToMaybe(),
                    Volume = 1.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        Resolution = new Resolution(1920, 1080).ToMaybe(),
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                        PixelBitDepth = PixelBitDepth.TenBits.ToMaybe(),
                    }.ToMaybe(),
                }
            },
        };
        #endregion

        #region public methods
        /// <summary>
        /// Creates the media metadata test model.
        /// </summary>
        /// <returns>An IEnumerable of key-value pairs of names to media metadata objects</returns>
        public static IEnumerable<KeyValuePair<IEnumerable<string>, MediaMetadata>> CreateMediaMetadataTestModel()
        {
            return InputToMediaMetadataMap.Value;
        }
        #endregion

        #region private methods
        private static IEnumerable<KeyValuePair<IEnumerable<string>, MediaMetadata>> CreateMapping(
            KeyValuePair<string, AudioCodec> audioTag,
            KeyValuePair<string, PixelBitDepth> pixelBitDepthTag,
            KeyValuePair<string, VideoCodec> videoCodecTag,
            KeyValuePair<string, VideoMedia> videoMediaTag,
            KeyValuePair<string, VideoMode> videoModeTag,
            KeyValuePair<string, Resolution> resolutionTag
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
                "01234567",
            };

            // This is just the entire permutation of all entries in the set of possible keys: (2 ^ 7) including the empty set
            for (int i = 0; i < 127; i++)
            {
                var tagList = new List<string>();
                var mediaMetadata = new MediaMetadata();

                for (int j = 0; j < 7; j++)
                {
                    var token = list[j];
                    int bitAt = (1 << j) & i;
                    if (bitAt > 0) // Remember this bitmask trick from the Algorithms Design Manual?
                    {
                        tagList.Add("[" + token + "]");
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
                                throw new InvalidOperationException("Invalid MediaMetadata model generation index");
                        }
                    }
                }

                yield return new KeyValuePair<IEnumerable<string>, MediaMetadata>(tagList, mediaMetadata);
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

        private static IEnumerable<KeyValuePair<IEnumerable<string>, MediaMetadata>> InitMediaMetadataTestModel()
        {
            var mapList = new List<IEnumerable<KeyValuePair<IEnumerable<string>, MediaMetadata>>>();
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
                                    new KeyValuePair<string, Resolution>(res1, new Resolution(1920, 1080))
                                );

                                var map2 = CreateMapping(
                                    audioTag,
                                    pixelBitDepthTag,
                                    videoCodecTag,
                                    videoMediaTag,
                                    videoModeTag,
                                    new KeyValuePair<string, Resolution>(res2, new Resolution(1280, 720))
                               );

                                var map3 = CreateMapping(
                                    audioTag,
                                    pixelBitDepthTag,
                                    videoCodecTag,
                                    videoMediaTag,
                                    videoModeTag,
                                    new KeyValuePair<string, Resolution>(res3, new Resolution(720, 480))
                                );

                                var map4 = CreateMapping(
                                    audioTag,
                                    pixelBitDepthTag,
                                    videoCodecTag,
                                    videoMediaTag,
                                    videoModeTag,
                                    new KeyValuePair<string, Resolution>(res4, new Resolution(1920, 1080))
                                );

                                var map5 = CreateMapping(
                                    audioTag,
                                    pixelBitDepthTag,
                                    videoCodecTag,
                                    videoMediaTag,
                                    videoModeTag,
                                    new KeyValuePair<string, Resolution>(res5, new Resolution(1280, 720))
                                );

                                var map6 = CreateMapping(
                                    audioTag,
                                    pixelBitDepthTag,
                                    videoCodecTag,
                                    videoMediaTag,
                                    videoModeTag,
                                    new KeyValuePair<string, Resolution>(res6, new Resolution(720, 480))
                                );

                                mapList.AddRange(new[] { map1, map2, map3, map4, map5, map6 });
                            }
                        }
                    }
                }
            }

            return from map in mapList
                   from kvp in map
                   select kvp;
        }
        #endregion
    }
}
