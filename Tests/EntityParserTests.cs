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
using Functional.Maybe;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTests
{

    /// <summary>
    /// Tests the <see cref="EntityParsers"/> class
    /// </summary>
    [TestClass]
    public sealed class EntityParserTests
    {
        #region OP/ED Parsers
        [TestMethod]
        public void ParseOpeningFromLine()
        {
            var inputMap = new Dictionary<string, EntityParsers.OPEDParseResult>
            {
                {
                    "[Final8]Mirai Nikki (Creditless OP3 - The Live World) (BD 10-bit 1280x720 x264 AAC)[8F8B757F].mkv",
                    new EntityParsers.OPEDParseResult
                    {
                        CreditlessPrefix = "Creditless".ToMaybe(),
                        OPEDToken = "OP".ToMaybe(),
                        SequenceNumber = 3.ToMaybe(),
                    }
                },
                {
                    "[Coalgirls] Rinne no Lagrange NCOP (1920x1080 Blu-Ray FLAC) [5C7D6075].mkv",
                    new EntityParsers.OPEDParseResult
                    {
                        CreditlessPrefix = "NC".ToMaybe(),
                        OPEDToken = "OP".ToMaybe(),
                        SequenceNumber = Maybe<int>.Nothing,
                    }
                },
                {
                    "[Coalgirls] Nekomonogatari OP (1920x1080 Blu-ray FLAC) [7F7E706B].mkv",
                    new EntityParsers.OPEDParseResult
                    {
                        CreditlessPrefix = Maybe<string>.Nothing,
                        OPEDToken = "OP".ToMaybe(),
                        SequenceNumber = Maybe<int>.Nothing,
                    }
                },
                {
                    "[Commie] Monogatari Series Second Season - NCOP 3 [BD 1080p AAC] [EDF5B72C].mkv",
                    new EntityParsers.OPEDParseResult
                    {
                        CreditlessPrefix = "NC".ToMaybe(),
                        OPEDToken = "OP".ToMaybe(),
                        SequenceNumber = 3.ToMaybe(),
                    }
                },
            };

            TestUtils.TestParser(inputMap, EntityParsers.ParseOpeningFromLine);
        }

        [TestMethod]
        public void ParseEndingFromLine()
        {
            var inputMap = new Dictionary<string, EntityParsers.OPEDParseResult>
            {
                {
                    "Utawarerumono Creditless ED [1080p,BluRay,x264] - THORA.mkv", 
                    new EntityParsers.OPEDParseResult
                    {
                        CreditlessPrefix = "Creditless".ToMaybe(),
                        OPEDToken = "ED".ToMaybe(),
                        SequenceNumber = Maybe<int>.Nothing,
                    }
                },
                {
                    "[Doki] Hoshizora e Kakaru Hashi - NCED (1920x1080 h264 BD FLAC) [9A6B46E4].mkv",
                    new EntityParsers.OPEDParseResult
                    {
                        CreditlessPrefix = "NC".ToMaybe(),
                        OPEDToken = "ED".ToMaybe(),
                        SequenceNumber = Maybe<int>.Nothing,
                    }
                },
                {
                    "[Final8]Mirai Nikki (Creditless ED3 - Happy End) (BD 10-bit 1920x1080 x264 FLAC)[2B9A87BA].mkv",
                    new EntityParsers.OPEDParseResult
                    {
                        CreditlessPrefix = "Creditless".ToMaybe(),
                        OPEDToken = "ED".ToMaybe(),
                        SequenceNumber = 3.ToMaybe(),
                    }
                },
                {
                    "[Coalgirls] Strike Witches 2 ED9 (1920x1080 Blu-Ray FLAC) [DE39EDD9].mkv",
                    new EntityParsers.OPEDParseResult
                    {
                        CreditlessPrefix = Maybe<string>.Nothing,
                        OPEDToken = "ED".ToMaybe(),
                        SequenceNumber = 9.ToMaybe(),
                    }
                },
                {
                    "[Coalgirls] Strike Witches 2 NCED9 (1920x1080 Blu-Ray FLAC) [DE39EDD9].mkv",
                    new EntityParsers.OPEDParseResult
                    {
                        CreditlessPrefix = "NC".ToMaybe(),
                        OPEDToken = "ED".ToMaybe(),
                        SequenceNumber = 9.ToMaybe(),
                    }
                },
            };

            TestUtils.TestParser(inputMap, EntityParsers.ParseEndingFromLine);
        }
        #endregion
    }
}
