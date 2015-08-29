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

        #region episode test data
        private static readonly IDictionary<string, IFansubEntity> EpisodeInputToEntityMap = new Dictionary<string, IFansubEntity>
        {
            {
                "[Coalgirls]_Cardcaptor_Sakura_01_(1440x1080_Blu-ray_FLAC)_[9B5FDB33].mkv",
                new FansubEpisodeEntity
                {
                    Group = "Coalgirls".ToMaybe(),
                    Series = "Cardcaptor Sakura".ToMaybe(),
                    EpisodeNumber = 1.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                        CRC32 = "9B5FDB33".ToMaybe(),
                        Resolution = new Resolution(1440, 1080).ToMaybe(),
                        UnusedTags = new[] {"Coalgirls"},
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[HorribleSubs] Akame ga Kill! - 01 [720p].mkv", 
                new FansubEpisodeEntity
                {
                    Group = "HorribleSubs".ToMaybe(),
                    EpisodeNumber = 1.ToMaybe(),
                    Series = "Akame ga Kill!".ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        VideoMode = VideoMode.SevenTwentyProgressive.ToMaybe(),
                        UnusedTags = new[] {"HorribleSubs"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[Vivid-Asenshi] Akame ga Kill - 01 [85196310].mkv", 
                new FansubEpisodeEntity
                {
                    Group = "Vivid-Asenshi".ToMaybe(),
                    Series = "Akame ga Kill".ToMaybe(),
                    EpisodeNumber = 1.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        CRC32 = "85196310".ToMaybe(),
                        UnusedTags = new[] {"Vivid-Asenshi"},

                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[Commie] Ore Monogatari!! - 01 [0C0EE0AB].mkv", 
                new FansubEpisodeEntity
                {
                    Group = "Commie".ToMaybe(),
                    Series = "Ore Monogatari!!".ToMaybe(),
                    EpisodeNumber = 1.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        CRC32 = "0C0EE0AB".ToMaybe(),
                        UnusedTags = new[] {"Commie"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[Yabai]_Spice_and_Wolf_II_-_05_[BD][E84477A3].mkv",
                new FansubEpisodeEntity
                {
                    Group = "Yabai".ToMaybe(),
                    Series = "Spice and Wolf II".ToMaybe(),
                    EpisodeNumber = 5.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        CRC32 = "E84477A3".ToMaybe(),
                        UnusedTags = new[] {"Yabai"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[Commie] Persona 4 - 25 [B87F03D9].mkv",
                new FansubEpisodeEntity
                {
                    Group = "Commie".ToMaybe(),
                    Series = "Persona 4".ToMaybe(),
                    EpisodeNumber = 25.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        CRC32 = "B87F03D9".ToMaybe(),
                        UnusedTags = new[] {"Commie"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
        };
        #endregion

        #region OVA test data
        private static readonly IDictionary<string, IFansubEntity> OriginalVideoInputToEntityMap = new Dictionary<string, IFansubEntity>
        {
            {
                "[SFW] Mahou Sensei Negima! OAD ~Mou Hitotsu no Sekai~ 02 [DVD][87942D8A].mkv", 
                new FansubOriginalAnimationEntity
                {
                    Group = "SFW".ToMaybe(),
                    Series = "Mahou Sensei Negima!".ToMaybe(),
                    Type = FansubOriginalAnimationEntity.ReleaseType.OAD.ToMaybe(),
                    Title = "~Mou Hitotsu no Sekai~".ToMaybe(),
                    EpisodeNumber = 2.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.DVD.ToMaybe(),
                        CRC32 = "87942D8A".ToMaybe(),
                        UnusedTags = new[] {"SFW"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[Hatsuyuki]_Noragami_OAD_-_01_[10bit][848x480][FDCA30AA].mkv",
                new FansubOriginalAnimationEntity
                {
                    Group = "Hatsuyuki".ToMaybe(),
                    Series = "Noragami".ToMaybe(),
                    Type = FansubOriginalAnimationEntity.ReleaseType.OAD.ToMaybe(),
                    EpisodeNumber = 1.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        PixelBitDepth = PixelBitDepth.TenBits.ToMaybe(),
                        Resolution = new Resolution(848, 480).ToMaybe(),
                        CRC32 = "FDCA30AA".ToMaybe(),
                        UnusedTags = new[] {"Hatsuyuki"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[SubDESU]_R-15_OAD_(DVD_720x480_x264_AAC)_[7D1F1EDF].mkv",
                new FansubOriginalAnimationEntity
                {
                    Group = "SubDESU".ToMaybe(),
                    Series = "R-15".ToMaybe(),
                    Type = FansubOriginalAnimationEntity.ReleaseType.OAD.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.DVD.ToMaybe(),
                        Resolution = new Resolution(720, 480).ToMaybe(),
                        VideoCodec = VideoCodec.H264.ToMaybe(),
                        AudioCodec = AudioCodec.AAC.ToMaybe(),
                        CRC32 = "7D1F1EDF".ToMaybe(),
                        UnusedTags = new[] {"SubDESU"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[Chihiro]_Hitsugi_no_Chaika_-_OAD_[1280x720_Blu-ray_AAC][7ECAA5A1].mkv", 
                new FansubOriginalAnimationEntity
                {
                    Group = "Chihiro".ToMaybe(),
                    Series = "Hitsugi no Chaika".ToMaybe(),
                    Type = FansubOriginalAnimationEntity.ReleaseType.OAD.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        Resolution = new Resolution(1280, 720).ToMaybe(),
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        AudioCodec = AudioCodec.AAC.ToMaybe(),
                        CRC32 = "7ECAA5A1".ToMaybe(),
                        UnusedTags = new[] {"Chihiro"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[Saizen]_Attack_on_Titan_-_OAD_1_[06EA79B8].mkv", 
                new FansubOriginalAnimationEntity
                {
                    Group = "Saizen".ToMaybe(),
                    Series = "Attack on Titan".ToMaybe(),
                    Type = FansubOriginalAnimationEntity.ReleaseType.OAD.ToMaybe(),
                    EpisodeNumber = 1.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        CRC32 = "06EA79B8".ToMaybe(),
                        UnusedTags = new[] {"Saizen"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[gg]_Joshiraku_-_OAD_[869F6659].mkv", 
                new FansubOriginalAnimationEntity
                {
                    Group = "gg".ToMaybe(),
                    Series = "Joshiraku".ToMaybe(),
                    Type = FansubOriginalAnimationEntity.ReleaseType.OAD.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        CRC32 = "869F6659".ToMaybe(),
                        UnusedTags = new[] {"gg"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[WCP] Nisekoi OAD 3 - Bath House & Service [576p][BCBBA0B2].mkv", 
                new FansubOriginalAnimationEntity
                {
                    Group = "WCP".ToMaybe(),
                    Series = "Nisekoi".ToMaybe(),
                    Type = FansubOriginalAnimationEntity.ReleaseType.OAD.ToMaybe(),
                    EpisodeNumber = 3.ToMaybe(),
                    Title = "Bath House & Service".ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        VideoMode = VideoMode.FiveSeventySixProgressive.ToMaybe(),
                        CRC32 = "BCBBA0B2".ToMaybe(),
                        UnusedTags = new[] {"WCP"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[AniYoshi]_School_Days_OVA_-_Valentine_Days_(DVD)_[5DE6B483].mkv", 
                new FansubOriginalAnimationEntity
                {
                    Group = "AniYoshi".ToMaybe(),
                    Series = "School Days".ToMaybe(),
                    Type = FansubOriginalAnimationEntity.ReleaseType.OVA.ToMaybe(),
                    Title = "Valentine Days".ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.DVD.ToMaybe(),
                        CRC32 = "5DE6B483".ToMaybe(),
                        UnusedTags = new[] {"AniYoshi"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[Coalgirls]_Valkyria_Chronicles_OVA_08_(1920x1080_Blu-Ray_FLAC)_[F015CD71].mkv", 
                new FansubOriginalAnimationEntity
                {
                    Group = "Coalgirls".ToMaybe(),
                    Series = "Valkyria Chronicles".ToMaybe(),
                    Type = FansubOriginalAnimationEntity.ReleaseType.OVA.ToMaybe(),
                    EpisodeNumber = 8.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        Resolution = new Resolution(1920, 1080).ToMaybe(),
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                        CRC32 = "F015CD71".ToMaybe(),
                        UnusedTags = new[] {"Coalgirls"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[Ayako-Himatsubushi] Seitokai Yakuindomo OVA - 01v2 [DVD][480p][EF82D238].mkv", 
                new FansubOriginalAnimationEntity
                {
                    Group = "Ayako-Himatsubushi".ToMaybe(),
                    Series = "Seitokai Yakuindomo".ToMaybe(),
                    Type = FansubOriginalAnimationEntity.ReleaseType.OVA.ToMaybe(),
                    EpisodeNumber = 1.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.DVD.ToMaybe(),
                        VideoMode = VideoMode.FourEightyProgressive.ToMaybe(),
                        CRC32 = "EF82D238".ToMaybe(),
                        UnusedTags = new[] {"Ayako-Himatsubushi"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[TastyMelon] Black Lagoon OVA - Roberta's Blood Trail - 04 [BD][480p][926257C1].mkv", 
                new FansubOriginalAnimationEntity
                {
                    Group = "TastyMelon".ToMaybe(),
                    Series = "Black Lagoon".ToMaybe(),
                    Type = FansubOriginalAnimationEntity.ReleaseType.OVA.ToMaybe(),
                    Title = "Roberta's Blood Trail".ToMaybe(),
                    EpisodeNumber = 4.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        VideoMode = VideoMode.FourEightyProgressive.ToMaybe(),
                        CRC32 = "926257C1".ToMaybe(),
                        UnusedTags = new[] {"TastyMelon"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
        };
        #endregion

        #region OP/ED Test Data
        private static readonly IDictionary<string, IFansubEntity> OpeningEndingInputToEntityMap = new Dictionary<string, IFansubEntity>
        {
            {
                "[FFF] Hyakka Ryouran Samurai Bride - ED06 [BD][1080p-FLAC][2218ADB6].mkv", 
                new FansubOPEDEntity
                {
                    Group = "FFF".ToMaybe(),
                    Series = "Hyakka Ryouran Samurai Bride".ToMaybe(),
                    Part = FansubOPEDEntity.Segment.ED.ToMaybe(),
                    SequenceNumber = 6.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        VideoMode = VideoMode.TenEightyProgressive.ToMaybe(),
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                        CRC32 = "2218ADB6".ToMaybe(),
                        UnusedTags = new[] {"FFF"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[Ryuumaru] Hyakka Ryouran Samurai Girls - ED (Clean) [1080p - Bluray][DB8185D1].mkv", 
                new FansubOPEDEntity
                {
                    Group = "Ryuumaru".ToMaybe(),
                    Series = "Hyakka Ryouran Samurai Girls".ToMaybe(),
                    Part = FansubOPEDEntity.Segment.ED.ToMaybe(),
                    NoCredits = true,
                    Metadata = new MediaMetadata
                    {
                        VideoMode = VideoMode.TenEightyProgressive.ToMaybe(),
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        CRC32 = "DB8185D1".ToMaybe(),
                        UnusedTags = new[] {"Ryuumaru", "Clean"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[Ryuumaru] Hyakka Ryouran Samurai Girls - OP (Clean) [1080p - Bluray][DB8185D1].mkv", 
                new FansubOPEDEntity
                {
                    Group = "Ryuumaru".ToMaybe(),
                    Series = "Hyakka Ryouran Samurai Girls".ToMaybe(),
                    Part = FansubOPEDEntity.Segment.OP.ToMaybe(),
                    NoCredits = true,
                    Metadata = new MediaMetadata
                    {
                        VideoMode = VideoMode.TenEightyProgressive.ToMaybe(),
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        CRC32 = "DB8185D1".ToMaybe(),
                        UnusedTags = new[] {"Ryuumaru", "Clean"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[Doki] GJ-bu - NCED 2 (1920x1080 Hi10P BD FLAC) [71E9D167].mkv", 
                new FansubOPEDEntity
                {
                    Group = "Doki".ToMaybe(),
                    Series = "GJ-bu".ToMaybe(),
                    Part = FansubOPEDEntity.Segment.ED.ToMaybe(),
                    SequenceNumber = 2.ToMaybe(),
                    NoCredits = true,
                    Metadata = new MediaMetadata
                    {
                        Resolution = new Resolution(1920, 1080).ToMaybe(),
                        PixelBitDepth = PixelBitDepth.TenBits.ToMaybe(),
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                        CRC32 = "71E9D167".ToMaybe(),
                        UnusedTags = new[] {"Doki"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe()
                }
            },
            {
                "[Underwater-FFF] Saki Achiga-hen - Episode of Side-A - ED02 [BD][1080p-FLAC][280EFFB2].mkv", 
                new FansubOPEDEntity
                {
                    Group = "Underwater-FFF".ToMaybe(),
                    Series = "Saki Achiga-hen - Episode of Side-A".ToMaybe(),
                    Part = FansubOPEDEntity.Segment.ED.ToMaybe(),
                    SequenceNumber = 2.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        VideoMode = VideoMode.TenEightyProgressive.ToMaybe(),
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                        CRC32 = "280EFFB2".ToMaybe(),
                        UnusedTags = new[] {"Underwater-FFF"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[Commie] Yuyushiki - NCED 09 [BD 720p AAC] [E291A643].mkv",
                new FansubOPEDEntity
                {
                    Group = "Commie".ToMaybe(),
                    Series = "Yuyushiki".ToMaybe(),
                    Part = FansubOPEDEntity.Segment.ED.ToMaybe(),
                    SequenceNumber = 9.ToMaybe(),
                    NoCredits = true,
                    Metadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        VideoMode = VideoMode.SevenTwentyProgressive.ToMaybe(),
                        AudioCodec = AudioCodec.AAC.ToMaybe(),
                        CRC32 = "E291A643".ToMaybe(),
                        UnusedTags = new[] {"Commie"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[UTW]_Shinsekai_Yori_-_Creditless_ED2_[BD][h264-1080p_FLAC][E3C12E42].mkv", 
                new FansubOPEDEntity
                {
                    Group = "UTW".ToMaybe(),
                    Series = "Shinsekai Yori".ToMaybe(),
                    NoCredits = true,
                    Part = FansubOPEDEntity.Segment.ED.ToMaybe(),
                    SequenceNumber = 2.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        VideoCodec = VideoCodec.H264.ToMaybe(),
                        VideoMode = VideoMode.TenEightyProgressive.ToMaybe(),
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                        CRC32 = "E3C12E42".ToMaybe(),
                        UnusedTags = new[] {"UTW"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[Coalgirls]_C3-Cube_x_Cursed_x_Curious_NCED_(1920x1080_Blu-Ray_FLAC)_[50450AA5].mkv", 
                new FansubOPEDEntity
                {
                    Group = "Coalgirls".ToMaybe(),
                    Series = "C3-Cube x Cursed x Curious".ToMaybe(),
                    NoCredits = true,
                    Part = FansubOPEDEntity.Segment.ED.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        Resolution = new Resolution(1920, 1080).ToMaybe(),
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                        CRC32 = "50450AA5".ToMaybe(),
                        UnusedTags = new[] {"Coalgirls"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[Coalgirls]_Valkyria_Chronicles_OP1_-_alt_(1920x1080_Blu-Ray_FLAC)_[414F6192].mkv", 
                new FansubOPEDEntity
                {
                    Group = "Coalgirls".ToMaybe(),
                    Series = "Valkyria Chronicles".ToMaybe(),
                    Part = FansubOPEDEntity.Segment.OP.ToMaybe(),
                    SequenceNumber = 1.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        Resolution = new Resolution(1920, 1080).ToMaybe(),
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                        CRC32 = "414F6192".ToMaybe(),
                        UnusedTags = new[] {"Coalgirls"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[Coalgirls]_Tokyo_Ghoul_NCOP1_(1920x1080_Blu-ray_FLAC)_[D91E552D].mkv", 
                new FansubOPEDEntity
                {
                    Group = "Coalgirls".ToMaybe(),
                    Series = "Tokyo Ghoul".ToMaybe(),
                    NoCredits = true,
                    Part = FansubOPEDEntity.Segment.OP.ToMaybe(),
                    SequenceNumber = 1.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        Resolution = new Resolution(1920, 1080).ToMaybe(),
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                        CRC32 = "D91E552D".ToMaybe(),
                        UnusedTags = new[] {"Coalgirls"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[WhyNot] Steins;Gate - NCOP [BD 720p AAC][1B65071D].mkv", 
                new FansubOPEDEntity
                {
                    Group = "WhyNot".ToMaybe(),
                    Series = "Steins;Gate".ToMaybe(),
                    NoCredits = true,
                    Part = FansubOPEDEntity.Segment.OP.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        VideoMode = VideoMode.SevenTwentyProgressive.ToMaybe(),
                        AudioCodec = AudioCodec.AAC.ToMaybe(),
                        CRC32 = "1B65071D".ToMaybe(),
                        UnusedTags = new[] {"WhyNot"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[Final8]Mirai Nikki (Creditless OP3 - The Live World) (BD 10-bit 1280x720 x264 AAC)[8F8B757F].mkv", 
                new FansubOPEDEntity
                {
                    Group = "Final8".ToMaybe(),
                    Series = "Mirai Nikki".ToMaybe(),
                    NoCredits = true,
                    Part = FansubOPEDEntity.Segment.OP.ToMaybe(),
                    SequenceNumber = 3.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        PixelBitDepth = PixelBitDepth.TenBits.ToMaybe(),
                        Resolution = new Resolution(1280, 720).ToMaybe(),
                        VideoCodec = VideoCodec.H264.ToMaybe(),
                        AudioCodec = AudioCodec.AAC.ToMaybe(),
                        CRC32 = "8F8B757F".ToMaybe(),
                        UnusedTags = new[] {"Final8", "Creditless OP3 - The Live World"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[joseole99] Gurren Lagann - NCOP3v2 (853x480 h264 AC3 AC3) [956587FD].mkv", 
                new FansubOPEDEntity
                {
                    Group = "joseole99".ToMaybe(),
                    Series = "Gurren Lagann".ToMaybe(),
                    NoCredits = true,
                    Part = FansubOPEDEntity.Segment.OP.ToMaybe(),
                    SequenceNumber = 3.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        Resolution = new Resolution(853, 480).ToMaybe(),
                        VideoCodec = VideoCodec.H264.ToMaybe(),
                        AudioCodec = AudioCodec.AC3.ToMaybe(),
                        CRC32 = "956587FD".ToMaybe(),
                        UnusedTags = new[] {"joseole99"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
            {
                "[Doki] Yuru Yuri 2 - NCOP 2v2 (1920x1080 Hi10P BD FLAC) [8CABBF62].mkv",
                new FansubOPEDEntity
                {
                    Group = "Doki".ToMaybe(),
                    Series = "Yuru Yuri 2".ToMaybe(),
                    NoCredits = true,
                    Part = FansubOPEDEntity.Segment.OP.ToMaybe(),
                    SequenceNumber = 2.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                        Resolution = new Resolution(1920, 1080).ToMaybe(),
                        PixelBitDepth = PixelBitDepth.TenBits.ToMaybe(),
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        CRC32 = "8CABBF62".ToMaybe(),
                        UnusedTags = new[] {"Doki"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe(),
                }
            },
        };
        #endregion

        #region directory test data
        private static readonly IDictionary<string, IFansubEntity> DirectoryInputToEntityMap = new Dictionary<string, IFansubEntity>
        {
            {
                "Kokoro Connect (2012) [Doki-Chihiro][1920x1080 Hi10P BD FLAC]", 
                new FansubDirectoryEntity 
                { 
                    Group = "Doki-Chihiro".ToMaybe(), 
                    Series = "Kokoro Connect".ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                        PixelBitDepth = PixelBitDepth.TenBits.ToMaybe(),
                        Resolution = new Resolution(1920, 1080).ToMaybe(),
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        UnusedTags = new[] {"2012", "Doki-Chihiro"},
                    }.ToMaybe(),
                }
            },
            {
                "[Vivid] The World God Only Knows - Goddesses Arc [BD 1080p FLAC]", 
                new FansubDirectoryEntity
                {
                    Group = "Vivid".ToMaybe(),
                    Series = "The World God Only Knows - Goddesses Arc".ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        VideoMode = VideoMode.TenEightyProgressive.ToMaybe(),
                        UnusedTags = new[] {"Vivid"},
                    }.ToMaybe(),
                }
            },
            {
                "[BlurayDesuYo] Amagi Brilliant Park - Vol. 1 (BD 1920x1080 10bit FLAC)",
                new FansubDirectoryEntity
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
                        UnusedTags = new[] {"BlurayDesuYo"},
                    }.ToMaybe(),
                }
            },
            {
                "[Coalgirls]_Amagi_Brilliant_Park_01-02_(1920x1080_Blu-Ray_FLAC)",
                new FansubDirectoryEntity
                {
                    Group = "Coalgirls".ToMaybe(),
                    Series = "Amagi Brilliant Park".ToMaybe(),
                    EpisodeRange = Tuple.Create(1, 2).ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        Resolution = new Resolution(1920, 1080).ToMaybe(),
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                        UnusedTags = new[] {"Coalgirls"},
                    }.ToMaybe(),
                }
            },
            {
                "[FFF] Medaka Box Abnormal - Vol.03 [BD][1080p-FLAC]", 
                new FansubDirectoryEntity
                {
                    Group = "FFF".ToMaybe(),
                    Series = "Medaka Box Abnormal".ToMaybe(),
                    Volume = 3.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        VideoMode = VideoMode.TenEightyProgressive.ToMaybe(),
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                        UnusedTags = new[] {"FFF"},
                    }.ToMaybe(),
                }
            },
            {
                "[Tsundere] Fate Kaleid Prisma Illya 2wei - 01-02 [BDRip h264 1920x1080 10bit FLAC]",
                new FansubDirectoryEntity
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
                        UnusedTags = new[] {"Tsundere"},
                    }.ToMaybe(),
                }
            },
            {
                "[Elysium]Spice.and.Wolf.II(BD.1080p.FLAC)", 
                new FansubDirectoryEntity
                {
                    Group = "Elysium".ToMaybe(),
                    Series = "Spice and Wolf II".ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        VideoMode = VideoMode.TenEightyProgressive.ToMaybe(),
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                        UnusedTags = new[] {"Elysium"},
                    }.ToMaybe(),
                }
            },
            {
                "[Yabai]_Spice_and_Wolf_II_[BD]", 
                new FansubDirectoryEntity
                {
                    Group = "Yabai".ToMaybe(),
                    Series = "Spice and Wolf II".ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        UnusedTags = new[] {"Yabai"},
                    }.ToMaybe(),
                }
            },
            {
                "[Zurako] Spice and Wolf II (BD 1080p AAC)",
                new FansubDirectoryEntity
                {
                    Group = "Zurako".ToMaybe(),
                    Series = "Spice and Wolf II".ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        VideoMode = VideoMode.TenEightyProgressive.ToMaybe(),
                        AudioCodec = AudioCodec.AAC.ToMaybe(),
                        UnusedTags = new[] {"Zurako"},
                    }.ToMaybe(),
                }
            },
            {
                "[UTW]_Fate_stay_night_Unlimited_Blade_Works_-_00-12_[BD][h264-1080p][FLAC]",
                new FansubDirectoryEntity
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
                        UnusedTags = new[] {"UTW"},
                    }.ToMaybe(),
                }
            },
            {
                "[Commie] Chihayafuru 2 - Volume 1 [BD 720p AAC]", 
                new FansubDirectoryEntity
                {
                    Group = "Commie".ToMaybe(),
                    Series = "Chihayafuru 2".ToMaybe(),
                    Volume = 1.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        VideoMode = VideoMode.SevenTwentyProgressive.ToMaybe(),
                        AudioCodec = AudioCodec.AAC.ToMaybe(),
                        UnusedTags = new[] {"Commie"},
                    }.ToMaybe(),
                }
            },
            {
                "[Coalgirls]_Hyouka_(1920x1080_Blu-Ray_FLAC)", 
                new FansubDirectoryEntity
                {
                    Group = "Coalgirls".ToMaybe(),
                    Series = "Hyouka".ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        Resolution = new Resolution(1920, 1080).ToMaybe(),
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                        UnusedTags = new[] {"Coalgirls"},
                    }.ToMaybe(),
                }
            },
            {
                "[Doki] Mahouka Koukou no Rettousei - Vol 1 (1920x1080 Hi10P BD FLAC)", 
                new FansubDirectoryEntity
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
                        UnusedTags = new[] {"Doki"},
                    }.ToMaybe(),
                }
            },
        };
        #endregion

        #region movie test data
        private static readonly IDictionary<string, IFansubEntity> MovieInputToEntityMap = new Dictionary<string, IFansubEntity>
        {
            {
                "[Coalgirls]_Magical_Girl_Madoka_Magica_the_Movie_III_-_Rebellion_(1920x1080_Blu-ray_FLAC)_[557238A8].mkv",
                new FansubMovieEntity
                {
                    Group = "Coalgirls".ToMaybe(),
                    Series = "Magical Girl Madoka Magica the Movie".ToMaybe(),
                    MovieNumber = 3.ToMaybe(),
                    Subtitle = "Rebellion".ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        Resolution = new Resolution(1920, 1080).ToMaybe(),
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                        CRC32 = "557238A8".ToMaybe(),
                        UnusedTags = new[] {"Coalgirls"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe()
                }
            },
            {
                "[Commie] Persona 3 the Movie #2 - Midsummer Knight's Dream [BD 720p AAC] [DDB2BF3C].mkv",
                new FansubMovieEntity
                {
                    Group = "Commie".ToMaybe(),
                    Series = "Persona 3 the Movie".ToMaybe(),
                    MovieNumber = 2.ToMaybe(),
                    Subtitle = "Midsummer Knight's Dream".ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        VideoMode = VideoMode.SevenTwentyProgressive.ToMaybe(),
                        AudioCodec = AudioCodec.AAC.ToMaybe(),
                        CRC32 = "DDB2BF3C".ToMaybe(),
                        UnusedTags = new[] {"Commie"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe()
                }
            },
            {
                "[Commie] Persona 3 the Movie #1 - Spring of Birth [BD 720p AAC] [1C7DAA54].mkv",
                new FansubMovieEntity
                {
                    Group = "Commie".ToMaybe(),
                    Series = "Persona 3 the Movie".ToMaybe(),
                    MovieNumber = 1.ToMaybe(),
                    Subtitle = "Spring of Birth".ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        VideoMode = VideoMode.SevenTwentyProgressive.ToMaybe(),
                        AudioCodec = AudioCodec.AAC.ToMaybe(),
                        CRC32 = "1C7DAA54".ToMaybe(),
                        UnusedTags = new[] {"Commie"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe()
                }
            },
            {
                "[Doki] Suzumiya Haruhi no Shoushitsu (1920x1080 Hi10P BD FLAC) [3F1D19CD].mkv",
                new FansubMovieEntity
                {
                    Group = "Doki".ToMaybe(),
                    Series = "Suzumiya Haruhi no Shoushitsu".ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        Resolution = new Resolution(1920, 1080).ToMaybe(),
                        PixelBitDepth = PixelBitDepth.TenBits.ToMaybe(),
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        AudioCodec = AudioCodec.FLAC.ToMaybe(),
                        CRC32 = "3F1D19CD".ToMaybe(),
                        UnusedTags = new[] {"Doki"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe()
                }
            },
            {
                "[Elysium]Rin.Daughters.of.Mnemosyne.EP4(BD.720p.AAC)[47ADB435].mkv",
                new FansubMovieEntity
                {
                    Group = "Elysium".ToMaybe(),
                    Series = "Rin Daughters of Mnemosyne".ToMaybe(),
                    MovieNumber = 4.ToMaybe(),
                    Metadata = new MediaMetadata
                    {
                        VideoMedia = VideoMedia.Bluray.ToMaybe(),
                        VideoMode = VideoMode.SevenTwentyProgressive.ToMaybe(),
                        AudioCodec = AudioCodec.AAC.ToMaybe(),
                        CRC32 = "47ADB435".ToMaybe(),
                        UnusedTags = new[] {"Elysium"},
                    }.ToMaybe(),
                    Extension = ".mkv".ToMaybe()
                }
            },
        };
        #endregion
        #endregion

        #region public properties
        /// <summary>
        /// The Opening/Ending test model
        /// </summary>
        public static IEnumerable<KeyValuePair<string, IFansubEntity>> OpeningEndingTestModel
        {
            get { return OpeningEndingInputToEntityMap; }
        }

        /// <summary>
        /// The Directory test model
        /// </summary>
        public static IEnumerable<KeyValuePair<string, IFansubEntity>> DirectoryTestModel
        {
            get { return DirectoryInputToEntityMap; }
        }

        /// <summary>
        /// The OVA teset model
        /// </summary>
        public static IEnumerable<KeyValuePair<string, IFansubEntity>> OriginalAnimationTestModel
        {
            get { return OriginalVideoInputToEntityMap; }
        }

        /// <summary>
        /// The episode test model
        /// </summary>
        public static IEnumerable<KeyValuePair<string, IFansubEntity>> EpisodeTestModel
        {
            get { return EpisodeInputToEntityMap; }
        }

        /// <summary>
        /// The movie test model
        /// </summary>
        public static IEnumerable<KeyValuePair<string, IFansubEntity>> MovieTestModel
        {
            get { return MovieInputToEntityMap; }
        }
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
