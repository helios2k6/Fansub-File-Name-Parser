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
using Functional.Maybe;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprache;
using System;
using System.Collections.Generic;

namespace UnitTests.Model.Parsers
{
    [TestClass]
    public class BaseParsersTests
    {
        [TestMethod]
        public void AllTags()
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

            TestUtils.TestMultiTokenParse(inputMap, BaseParsers.AllTags);
        }

        [TestMethod]
        public void SeparateTagsFromMainContent()
        {
            var inputMap = new Dictionary<string, BaseParsers.SeparatedParseResult>
            {
                {
                    "Kokoro Connect (2012) [Doki-Chihiro][1920x1080 Hi10P BD FLAC]", 
                    new BaseParsers.SeparatedParseResult(
                        Maybe<string>.Nothing, 
                        "Kokoro Connect".ToMaybe(), 
                        new[] {"2012", "Doki-Chihiro", "1920x1080 Hi10P BD FLAC"}
                    )
                },
                {
                    "[Doki] Akame ga Kill! - Vol 1 (1920x1080 Hi10P BD FLAC)", 
                    new BaseParsers.SeparatedParseResult(
                        "Doki".ToMaybe(), 
                        "Akame ga Kill! - Vol 1".ToMaybe(), 
                        new[] {"1920x1080 Hi10P BD FLAC"}
                    )
                },
                {
                    "[Doki] GJ-bu - 01v2 (1920x1080 Hi10P BD FLAC) [AB38621D].mkv", 
                    new BaseParsers.SeparatedParseResult(
                        "Doki".ToMaybe(), 
                        "GJ-bu - 01v2".ToMaybe(), 
                        new[] {"1920x1080 Hi10P BD FLAC", "AB38621D"}
                    )
                },
                {
                    "[Ayu]_Minami-ke_-_01v2_[720p_H264_MP3][F3CF6D14].mkv", 
                    new BaseParsers.SeparatedParseResult(
                        "Ayu".ToMaybe(), 
                        "_Minami-ke_-_01v2_".ToMaybe(), 
                        new[] {"720p_H264_MP3", "F3CF6D14"}
                    )
                },
                {
                    "(B-A)Devilman_Lady_-_01_(2E088B82).mkv", 
                    new BaseParsers.SeparatedParseResult(
                        "B-A".ToMaybe(), 
                        "Devilman_Lady_-_01_".ToMaybe(), 
                        new[] {"2E088B82"}
                    )
                },
            };

            TestUtils.TestParser(inputMap, BaseParsers.SeparateTagsFromMainContent);
        }
    }
}