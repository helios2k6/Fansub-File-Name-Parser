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
using FansubFileNameParser.Entity;
using FansubFileNameParser.Entity.Directory;
using FansubFileNameParser.Entity.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprache;
using System;
using System.Collections.Generic;
using UnitTests.Models;

namespace UnitTests
{
    /// <summary>
    /// Tests the <see cref="EntityParsers"/> class
    /// </summary>
    [TestClass]
    public sealed class EntityParserTests
    {
        private static void TestParserHelper<TResultType>(
            IEnumerable<KeyValuePair<string, IFansubEntity>> model
        )
        {
            foreach (var stringToModel in model)
            {
                var fansubString = stringToModel.Key;
                var expectedParseResult = stringToModel.Value;
                var parseResult = EntityParsers.EntityParser.TryParse(fansubString);
                Assert.AreEqual<Type>(typeof(TResultType), expectedParseResult.GetType());
                Assert.IsTrue(parseResult.WasSuccessful);
                Assert.AreEqual<Type>(expectedParseResult.GetType(), parseResult.Value.GetType());
                Assert.AreEqual<TResultType>((TResultType)expectedParseResult, (TResultType)parseResult.Value);
            }
        }

        [TestMethod]
        public void TestParseOPEDParser()
        {
            TestParserHelper<FansubOPEDEntity>(TestModel.OpeningEndingTestModel);
        }

        [TestMethod]
        public void TestParseDirectoryParser()
        {
            TestParserHelper<FansubDirectoryEntity>(TestModel.DirectoryTestModel);
        }

        [TestMethod]
        public void TestParseOVAParser()
        {
            TestParserHelper<FansubOriginalAnimationEntity>(TestModel.OriginalAnimationTestModel);
        }

        [TestMethod]
        public void TestEpisodeParser()
        {
            TestParserHelper<FansubEpisodeEntity>(TestModel.EpisodeTestModel);
        }
    }
}
