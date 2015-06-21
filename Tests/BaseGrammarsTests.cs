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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprache;
using System.Collections.Generic;

namespace UnitTests.Model.Grammars
{
    [TestClass]
    public sealed class BaseGrammarsTests
    {
        [TestMethod]
        public void DashSeparatorToken()
        {
            var inputOutputMap = new Dictionary<string, string>
            {
                {" - ", " - "}
            };

            TestUtils.TestParser(inputOutputMap, BaseGrammars.DashSeparatorToken);
        }

        [TestMethod]
        public void TagDeliminator()
        {
            var inputMap = new Dictionary<string, char>
            {
                {"[", '['},
                {"(", '('},
                {"]", ']'},
                {")", ')'},

            };

            TestUtils.TestParser(inputMap, BaseGrammars.TagDeliminator);
        }

        [TestMethod]
        public void MetaTagContent()
        {
            var inputMap = new Dictionary<string, string>
            {
                {"[test]", "test"},
                {"(test)", "test"},
                {"[test multiword]", "test multiword"},
                {"(test multiword)", "test multiword"},

            };

            TestUtils.TestParser(inputMap, BaseGrammars.MetaTagContent);
        }

        [TestMethod]
        public void MetaTagGroup()
        {
            var inputMap = new Dictionary<string, IEnumerable<string>>
            {
                {"[test][multiple][tags]", new[] {"test", "multiple", "tags"}},
                {"[test multiple][tags]", new[] {"test multiple", "tags"}},
                {"(test)(multiple)(tags)", new[] {"test", "multiple", "tags"}},
                {"(test multiple)(tags)", new[] {"test multiple", "tags"}},
                {"[test] [multiple] [tags] ", new[] {"test", "multiple", "tags"}},
                {" [test multiple] [tags] ", new[] {"test multiple", "tags"}},
                {" (test) (multiple) (tags) ", new[] {"test", "multiple", "tags"}},
                {" (test multiple) (tags) ", new[] {"test multiple", "tags"}},
                {"(test multiple) STUFF IN BETWEEN (tags) ", new[] {"test multiple"}},
            };

            TestUtils.TestMultiTokenParse(inputMap, BaseGrammars.MetaTagGroup);
        }

        [TestMethod]
        public void EpisodeNumber()
        {
            var inputOutputMap = new Dictionary<string, int>
            {
                {" 4 ", 4},
                {" 04 ", 4},
                {" 15 ", 15},
                {" 015 ", 15},
            };

            TestUtils.TestParser(inputOutputMap, BaseGrammars.EpisodeNumber);
        }

        [TestMethod]
        public void LineUpToEpisodeNumberToken()
        {
            var inputOutputMap = new Dictionary<string, string>
            {
                {"[Doki] Akame ga Kill! - 01 (1920x1080 Hi10P BD FLAC) [395609BF].mkv", "[Doki] Akame ga Kill! -"},
                {"[Coalgirls] Kill Me Baby 01 (1280x720_Blu-ray_FLAC) [1D51648A].mkv", "[Coalgirls] Kill Me Baby"},
                {"[Coalgirls] Kill Me Baby (1280x720 Blu-Ray FLAC)", "[Coalgirls] Kill Me Baby (1280x720 Blu-Ray FLAC)"},
            };

            TestUtils.TestParser(inputOutputMap, BaseGrammars.LineUpToEpisodeNumberToken);
        }

        [TestMethod]
        public void LineUpToLastDashSeparatorToken()
        {
            var inputOutputMap = new Dictionary<string, string>
            {
                {"[Tsundere] Fate Kaleid Prisma Illya 2wei - 01-02 [BDRip h264 1920x1080 10bit FLAC]", "[Tsundere] Fate Kaleid Prisma Illya 2wei"},
                {"[Elysium]Spice and Wolf II(BD 1080p FLAC)", "[Elysium]Spice and Wolf II(BD 1080p FLAC)"},
                {"[UTW] Fate stay night Unlimited Blade Works - 00-12 [BD][h264-1080p][FLAC]", "[UTW] Fate stay night Unlimited Blade Works"},
                {"[TastyMelon] Black Lagoon OVA - Roberta's Blood Trail - 04 [BD][480p][926257C1].mkv", "[TastyMelon] Black Lagoon OVA - Roberta's Blood Trail"},
            };

            TestUtils.TestParser(inputOutputMap, BaseGrammars.LineUpToLastDashSeparatorToken);
        }

        [TestMethod]
        public void LineUntilTagDeliminator()
        {
            var inputOutputMap = new Dictionary<string, string>
            {
                {"(hello", ""},
                {"hello(world", "hello"},
                {"helloworld(", "helloworld"},
                {"hello world(", "hello world"},
                {"[hello", ""},
                {"hello[world", "hello"},
                {"helloworld[", "helloworld"},
                {"hello world[", "hello world"}
            };

            TestUtils.TestParser(inputOutputMap, BaseGrammars.LineUpToTagDeliminator);
        }


        [TestMethod]
        public void ContentBetweenTagGroups()
        {
            var inputOutputMap = new Dictionary<string, string>
            {
                {"Kokoro Connect (2012) [Doki-Chihiro][1920x1080 Hi10P BD FLAC]", "Kokoro Connect"},
                {"[Vivid] The World God Only Knows - Goddesses Arc [BD 1080p FLAC]", "The World God Only Knows - Goddesses Arc"},
                {"[BlurayDesuYo] Amagi Brilliant Park - Vol. 1 (BD 1920x1080 10bit FLAC)", "Amagi Brilliant Park - Vol. 1"},
            };

            TestUtils.TestParser(inputOutputMap, BaseGrammars.ContentBetweenTagGroups);
        }

        [TestMethod]
        public void CollectTags()
        {
            var inputMap = new Dictionary<string, IEnumerable<string>>
            {
                {"[Doki] Akame ga Kill! - Vol 1 (1920x1080 Hi10P BD FLAC)", new[] {"Doki", "1920x1080 Hi10P BD FLAC"}},
                {"[HorribleSubs] Akame ga Kill! - 01 [720p].mkv", new[] {"HorribleSubs", "720p"}},
                {"[gg]_Ben-to_-_12_[33993A13].mkv", new[] {"gg", "33993A13"}},
                {"[Coalgirls]_Amagi_Brilliant_Park_01_(1920x1080_Blu-Ray_FLAC)_[54A49F15].mkv", new[] {"Coalgirls", "1920x1080_Blu-Ray_FLAC", "54A49F15"}},
                {"[TCL]_Claymore_01_[Blu-Ray][720p][13A866AC].mkv", new[] {"TCL", "Blu-Ray", "720p", "13A866AC"}},
                {"(B-A)Devilman_Lady_-_01_(2E088B82).mkv", new[] {"B-A", "2E088B82"}},
            };

            TestUtils.TestMultiTokenParse(inputMap, BaseGrammars.CollectTags);
        }

        [TestMethod]
        public void FileExtension()
        {
            var inputMap = new Dictionary<string, string>
            {
                {".avi", ".avi"},
                {".mkv", ".mkv"},
                {".mp4", ".mp4"},
                {".m2ts", ".m2ts"},
                {".ogg", ".ogg"},
                {".wmv", ".wmv"},
            };

            TestUtils.TestParser(inputMap, BaseGrammars.FileExtension);
        }

        [TestMethod]
        public void ReplaceDotsExceptMediaFileExtension()
        {
            var inputMap = new Dictionary<string, string>
            {
                {"[Elysium]Spice.and.Wolf.II(BD.1080p.FLAC)", "[Elysium]Spice and Wolf II(BD 1080p FLAC)"},
                {"[Elysium]Spice.and.Wolf.II(BD.1080p.FLAC).mkv", "[Elysium]Spice and Wolf II(BD 1080p FLAC).mkv"},
                {"[gg]_Binbougami_ga!_-_01_[D909F54C].mkv", "[gg]_Binbougami_ga!_-_01_[D909F54C].mkv"},
            };

            TestUtils.TestParser(inputMap, BaseGrammars.ReplaceDotsExceptMediaFileExtension);
        }

        [TestMethod]
        public void ReplaceUnderscores()
        {
            var inputMap = new Dictionary<string, string>
            {
                {"(B-A)Devilman_Lady_-_01_(2E088B82).mkv", "(B-A)Devilman Lady - 01 (2E088B82).mkv"},
                {"[Elysium]Spice.and.Wolf.II(BD.1080p.FLAC)", "[Elysium]Spice.and.Wolf.II(BD.1080p.FLAC)"},
                {"[Coalgirls]_Amagi_Brilliant_Park_01-02_(1920x1080_Blu-Ray_FLAC)", "[Coalgirls] Amagi Brilliant Park 01-02 (1920x1080 Blu-Ray FLAC)"},
            };

            TestUtils.TestParser(inputMap, BaseGrammars.ReplaceUnderscores);
        }

        [TestMethod]
        public void CleanString()
        {
            var inputMap = new Dictionary<string, string>
            {
                {"[Doki] Akame ga Kill! - Vol 1 (1920x1080 Hi10P BD FLAC)", "[Doki] Akame ga Kill! - Vol 1 (1920x1080 Hi10P BD FLAC)"},
                {"(B-A)Devilman_Lady_-_01_(2E088B82).mkv", "(B-A)Devilman Lady - 01 (2E088B82).mkv"},
                {"[Elysium]Spice.and.Wolf.II(BD.1080p.FLAC)", "[Elysium]Spice and Wolf II(BD 1080p FLAC)"},
                {"[gg].Binbougami_ga!_-.01_[D909F54C].mkv", "[gg] Binbougami ga! - 01 [D909F54C].mkv"},
                {"[gg]_Binbougami_ga!_-_01_[D909F54C].mkv", "[gg] Binbougami ga! - 01 [D909F54C].mkv"},
                {"[Coalgirls]_Amagi_Brilliant_Park_01-02_(1920x1080_Blu-Ray_FLAC)", "[Coalgirls] Amagi Brilliant Park 01-02 (1920x1080 Blu-Ray FLAC)"},
            };

            TestUtils.TestParser(inputMap, BaseGrammars.CleanInputString);
        }
    }
}
