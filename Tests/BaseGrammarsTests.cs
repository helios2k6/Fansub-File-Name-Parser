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
using System.Linq;
using System.Text;
using UnitTests;

namespace UnitTests.Model.Grammars
{
    [TestClass]
    public class BaseGrammarsTests
    {
        [TestMethod]
        public void DashParser()
        {
            var inputOutputMap = new Dictionary<string, string>
            {
                {"-", "-"},
                {"-A-", "-"},
                {"--", "--"}
            };

            TestUtils.TestParser(inputOutputMap, BaseGrammars.DashAtLeastOnce);
        }

        [TestMethod]
        public void LineParser()
        {
            var inputOutputMap = new Dictionary<string, string>
            {
                {"hello", "hello"},
                {" hello ", "hello"},
                {"hello world", "hello world"},
                {"hello world ", "hello world"},
                {" hello world ", "hello world"}
            };

            TestUtils.TestParser(inputOutputMap, BaseGrammars.Line);
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

            TestUtils.TestParser(inputOutputMap, BaseGrammars.OpenTagDeliminator);
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

            TestUtils.TestParser(inputOutputMap, BaseGrammars.ClosedTagDeliminator);
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

            TestUtils.TestParser(inputOutputMap, BaseGrammars.TagDeliminator);
        }

        [TestMethod]
        public void TagEnclosedText()
        {
            var inputMap = new Dictionary<string, string>
            {
                {"[test]", "test"},
                {"(test)", "test"},
                {"[test multiword]", "test multiword"},
                {"(test multiword)", "test multiword"},

            };

            TestUtils.TestParser(inputMap, BaseGrammars.TagEnclosedText);
        }

        [TestMethod]
        public void TagEnclosedTextWithDeliminator()
        {
            var inputMap = new Dictionary<string, string>
            {
                {"[test]", "[test]"},
                {"(test)", "(test)"},
                {"[test multiword]", "[test multiword]"},
                {"(test multiword)", "(test multiword)"},
            };

            TestUtils.TestParser(inputMap, BaseGrammars.TagEnclosedTextWithDeliminator);
        }

        [TestMethod]
        public void MultipleTagEnclsoedText()
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
            };

            TestUtils.TestMultiTokenParse(inputMap, BaseGrammars.MultipleTagEnclosedText);
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

            TestUtils.TestParser(inputOutputMap, BaseGrammars.LineExceptTagDeliminator);
        }

        [TestMethod]
        public void LinesSeparatedByDash()
        {
            var inputOutputMap = new Dictionary<string, IEnumerable<string>>
            {
                {"-hello world", new[] {"", "hello world"}},
                {" hello - world", new[] {"hello", "world"}},
                {"hello world", new[] {"hello world"}},
                {"hello world-", new[] {"hello world"}},
                {"-hello-world", new[] {"", "hello", "world"}},
                {"hello-world", new[] {"hello", "world"}}
            };

            TestUtils.TestMultiTokenParse(inputOutputMap, BaseGrammars.LinesSeparatedByDash);
        }
    }
}
