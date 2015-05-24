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
using FansubFileNameParser;
using Sprache;
using System.Collections.Generic;
using System.Text;

namespace UnitTests.Model.Grammars
{
    [TestClass]
    public class BaseGrammarsTests
    {
        private static void ParseWithMapHelper<T>(IDictionary<string, T> inputOutputMap, Parser<T> parser)
        {
            foreach (var t in inputOutputMap)
            {
                var result = parser.TryParse(t.Key);
                Assert.IsTrue(result.WasSuccessful);
                Assert.AreEqual(t.Value, result.Value);
            }
        }

        private static void ParseWithMapHelper<T>(IDictionary<string, T> inputOutputMap, Parser<IEnumerable<T>> parser)
        {
            foreach (var t in inputOutputMap)
            {
                var result = parser.TryParse(t.Key);
                Assert.IsTrue(result.WasSuccessful);
                var builder = new StringBuilder();
                foreach (var s in result.Value)
                {
                    builder.Append(s);
                }
                Assert.AreEqual(t.Value, builder.ToString());
            }
        }

        [TestMethod]
        public void DashParser()
        {
            var inputOutputMap = new Dictionary<string, string>
            {
                {"-", "-"},
                {"-A-", "-"},
                {"--", "--"}
            };

            ParseWithMapHelper(inputOutputMap, BaseGrammars.DashAtLeastOnce);
        }

        [TestMethod]
        public void LineParser()
        {
            var inputOutputMap = new Dictionary<string, string>
            {
                {"hello", "hello"},
                {" hello ", " hello "},
                {"hello world", "hello world"},
                {"hello world ", "hello world "},
                {" hello world ", " hello world "}
            };

            ParseWithMapHelper(inputOutputMap, BaseGrammars.Line);
        }

        [TestMethod]
        public void IdentifierParser()
        {
            var inputString = "name";
            var parseResult = BaseGrammars.Identifier.TryParse(inputString);

            Assert.IsTrue(parseResult.WasSuccessful);
            Assert.AreEqual(inputString, parseResult.Value);

            var inputString2 = "name__k";
            var parseResult2 = BaseGrammars.Identifier.TryParse(inputString2);

            Assert.IsTrue(parseResult2.WasSuccessful);
            Assert.AreEqual(inputString, parseResult2.Value);
        }

        [TestMethod]
        public void OpenTagDeliminatorParser()
        {
            var inputOutputMap = new Dictionary<string, char>
            {
                // Open parenthesis
                {"(", '('},
                {"(A)", '('},
                {"((", '('},
                
                // Open square bracket
                {"[", '['},
                {"[A]", '['},
                {"[[", '['},
            };

            ParseWithMapHelper(inputOutputMap, BaseGrammars.OpenTagDeliminator);
        }

        [TestMethod]
        public void ClosedTagDeliminatorParser()
        {
            var inputOutputMap = new Dictionary<string, char>
            {
                // Closed parenthesis
                {")", ')'},
                {")A)", ')'},
                {"))", ')'},
                
                // Closed square bracket
                {"]", ']'},
                {"]A]", ']'},
                {"]]", ']'},
            };

            ParseWithMapHelper(inputOutputMap, BaseGrammars.ClosedTagDeliminator);
        }

        [TestMethod]
        public void AnyTagDeliminatorParser()
        {
            var inputOutputMap = new Dictionary<string, char>
            {
                // Open parenthesis
                {"(", '('},
                {"(A)", '('},
                {"((", '('},
                
                // Open square bracket
                {"[", '['},
                {"[A]", '['},
                {"[[", '['},

                // Closed parenthesis
                {")", ')'},
                {")A)", ')'},
                {"))", ')'},
                
                // Closed square bracket
                {"]", ']'},
                {"]A]", ']'},
                {"]]", ']'},
            };

            ParseWithMapHelper(inputOutputMap, BaseGrammars.TagDeliminator);
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

            ParseWithMapHelper(inputOutputMap, BaseGrammars.LineUntilTagDeliminator);
        }

        [TestMethod]
        public void LinesSeparatedByDash()
        {
            var inputOutputMap = new Dictionary<string, string>
            {
                {"-hello world", "hello world"},
                {" hello - world", " hello  world"},
                {"hello world", "hello world"},
                {"hello world-", "hello world"},
                {"-hello-world", "helloworld"},
                {"hello-world", "helloworld"}
            };

            ParseWithMapHelper(inputOutputMap, BaseGrammars.LinesSeparatedByDash);
        }
    }
}
