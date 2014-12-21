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

using FansubFileNameParser;
using FansubFileNameParser.Metadata;
using System.Collections.Generic;

namespace UnitTests.Models
{
    public static class TestModel
    {
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
            {"[Lunar] Bleach - 05 v2 [F2C9454F].avi", new FansubFile("Lunar", "Bleach", 5, ".avi")}
        };

        /// <summary>
        /// Creates a new test model.
        /// </summary>
        /// <returns>A dictionary representing the test model</returns>
        public static IDictionary<string, FansubFile> CreateFansubFileTestModel()
        {
            return new Dictionary<string, FansubFile>(InputToFansubFileMap);
        }

        public static IDictionary<string, MediaMetadata> CreateMediaMetadataTestModel()
        {
            var mapResult = new Dictionary<string, MediaMetadata>();

            foreach(var audioTag in Tags.AudioTags)
            {
                foreach(var pixelBitDepthTag in Tags.PixelBitDepthTags)
                {
                    foreach(var videoCodecTag in Tags.VideoCodecTags)
                    {
                        foreach(var videoMediaTag in Tags.VideoMediaTags)
                        {
                            foreach(var videoModeTag in Tags.VideoModeTags)
                            {
                                //Generate crc32
                                var crc32 = "01234567";

                                //Generate resolution
                                var resolution1 = "1920x1080";
                                var resolution2 = "1280x720";
                                var resolution3 = "1920 x 1080";
                                var resolution4 = "1280 x 720";
                                var resolution5 = "720 x 480";
                                var resolution6 = "720x480";
                            }
                        }
                    }
                }
            }

            return mapResult;
        }
    }
}
