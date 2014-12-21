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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using FansubFileNameParser;
using Sprache;
using UnitTests.Models;

namespace UnitTests.Model.Grammars
{
    [TestClass]
    public class FansubFileParsersTests
    {
        [TestMethod]
        public void TryParseFansubFile()
        {
            var testModel = TestModel.CreateFansubFileTestModel();
            foreach (var k in testModel)
            {
                var file = FansubFileParsers.ParseFansubFile(k.Key);
                Assert.AreEqual(k.Value, file);
            }
        }

        [TestMethod]
        public void NormalizedFileNameParser()
        {
            var inputOutputMap = new Dictionary<string, FansubFile>
            {
                {"Hello (1).mkv", new FansubFile(string.Empty, "Hello", 1, ".mkv")},
                {"Hello-kitty (1).mkv", new FansubFile(string.Empty, "Hello-kitty", 1, ".mkv")}
            };

            foreach (var k in inputOutputMap)
            {
                var result = FansubFileParsers.NormalizedFileNameParser.TryParse(k.Key);
                Assert.IsTrue(result.WasSuccessful);
                Assert.AreEqual(k.Value, result.Value);
            }
        }
    }
}
