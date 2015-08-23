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
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public sealed class ExtraParserTests
    {
        [TestMethod]
        public void TestIntParser()
        {
            var inputOutputMap = new Dictionary<string, int>
            {
                {"0", 0},
                {"10", 10},
                {"01", 1},
            };

            TestUtils.TestParser(inputOutputMap, ExtraParsers.Int);
        }

        [TestMethod]
        public void TestImplode()
        {
            var multiCharParserImploded = Parse.AnyChar.Many().Implode<char, string>(string.Empty, (acc, ch) => string.Format("{0}{1}", acc, ch));
            var testString = "abcdef";

            var result = multiCharParserImploded.TryParse(testString);
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual<string>(testString, result.Value);
        }

        [TestMethod]
        public void TestContinueWith()
        {
            var antecedentParser = Parse.AnyChar;
            var continuationParser = Parse.AnyChar;
            var continuedParser = antecedentParser.ContinueWith(s => new string(s, 1), continuationParser);

            var result = continuationParser.TryParse("ab");
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual<char>('a', result.Value);

            var twoCharacterParser = Parse.AnyChar.Repeat(2).Text();
            var twoCharacterContinuedParser = twoCharacterParser.ContinueWith(twoCharacterParser);

            var secondResult = twoCharacterContinuedParser.TryParse("abc");
            Assert.IsTrue(secondResult.WasSuccessful);
            Assert.AreEqual<string>("ab", secondResult.Value);
        }

        [TestMethod]
        public void TestScanFor()
        {
            var excepter = Parse.String("TOKEN").Text();

            var result = ExtraParsers.ScanFor(excepter).TryParse("abcdTOKENefgh");
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual<string>("TOKEN", result.Value);
        }

        [TestMethod]
        public void TestLineUpTo()
        {
            var exceptor = Parse.String("TOKEN").Text();

            var result = ExtraParsers.LineUpTo(exceptor).TryParse("abcdTOKENefghTOKEN");
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual<string>("abcd", result.Value);
        }

        [TestMethod]
        public void TestResetInput()
        {
            var parser = Parse.String("TOKEN").Text();

            var compositeParser = from _ in parser.ResetInput()
                                  from s in parser
                                  select s;

            var result = compositeParser.TryParse("TOKEN");
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual<string>("TOKEN", result.Value);
        }

        [TestMethod]
        public void TestOptionalMaybe()
        {
            var parser = from f in Parse.Char('a').OptionalMaybe()
                         from s in Parse.Char('b').OptionalMaybe()
                         select f;

            var firstResult = parser.TryParse("b");
            Assert.IsTrue(firstResult.WasSuccessful);
            Assert.AreEqual<Maybe<char>>(Maybe<char>.Nothing, firstResult.Value);

            var secondResult = parser.TryParse("ac");
            Assert.IsTrue(secondResult.WasSuccessful);
            Assert.AreEqual<Maybe<char>>('a'.ToMaybe(), secondResult.Value);
        }

        [TestMethod]
        public void TestLast()
        {
            var lastNumberParser = Parse.Number.Last();

            var result = lastNumberParser.TryParse("1000");
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual<string>("1000", result.Value);

            var failedResult = lastNumberParser.TryParse("1000 2000");
            Assert.IsFalse(failedResult.WasSuccessful);
        }

        [TestMethod]
        public void TestFilter()
        {
            var filteredParser = ExtraParsers.Filter(Parse.Number);

            var result = filteredParser.TryParse("abc23efghij00");
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual<string>("abc efghij", result.Value);

            var secondResult = filteredParser.TryParse("1000");
            Assert.IsTrue(secondResult.WasSuccessful);
            Assert.AreEqual<string>(string.Empty, secondResult.Value);
        }

        [TestMethod]
        public void TestSetResultAsRemainder()
        {
            var remainderParser = from _ in Parse.Number.SetResultAsRemainder()
                                  from n in Parse.Number
                                  select n;

            var result = remainderParser.TryParse("500");
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual<string>("500", result.Value);
        }

        [TestMethod]
        public void TestBeginningOfLine()
        {
            var successResult = ExtraParsers.BeginnningOfLine.TryParse("abc");
            Assert.IsTrue(successResult.WasSuccessful);
            Assert.AreEqual<string>(string.Empty, successResult.Value);

            var failureParser = from _1 in Parse.AnyChar
                                from _2 in ExtraParsers.BeginnningOfLine
                                select _1;

            var failureResult = failureParser.TryParse("abc");
            Assert.IsFalse(failureResult.WasSuccessful);
        }

        [TestMethod]
        public void TestTrim()
        {
            var result1 = ExtraParsers.Trim.TryParse(" abc ");
            Assert.IsTrue(result1.WasSuccessful);
            Assert.AreEqual<string>("abc", result1.Value);

            var result2 = ExtraParsers.Trim.TryParse(" abc");
            Assert.IsTrue(result2.WasSuccessful);
            Assert.AreEqual<string>("abc", result2.Value);

            var result3 = ExtraParsers.Trim.TryParse("abc ");
            Assert.IsTrue(result3.WasSuccessful);
            Assert.AreEqual<string>("abc", result3.Value);

            var result4 = ExtraParsers.Trim.TryParse("abc");
            Assert.IsTrue(result4.WasSuccessful);
            Assert.AreEqual<string>("abc", result4.Value);
        }

        [TestMethod]
        public void TestAtLeastOneCharTrimmed()
        {
            var parser = Parse.AnyChar.Many().Text().AtLeastOneCharTrimmed();
            var successResult = parser.TryParse("abc");
            Assert.IsTrue(successResult.WasSuccessful);
            Assert.AreEqual("abc", successResult.Value);

            var failureResult = parser.TryParse("  ");
            Assert.IsFalse(failureResult.WasSuccessful);
        }

        [TestMethod]
        public void TestConsumeAllRemainingInput()
        {
            var parser = from _1 in Parse.AnyChar.ConsumeAllRemainingInput()
                         from _2 in Parse.LineTerminator
                         select _1;

            var result = parser.TryParse("abc");
            Assert.IsTrue(result.WasSuccessful);
            Assert.AreEqual<char>('a', result.Value);
        }

        [TestMethod]
        public void TestCutOut()
        {

        }
    }
}
