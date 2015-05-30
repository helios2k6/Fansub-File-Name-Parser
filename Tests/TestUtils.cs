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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTests
{
    /// <summary>
    /// Test utilties for grammar and parser tests
    /// </summary>
    internal static class TestUtils
    {
        /// <summary>
        /// Tests the parser against the map of input to its corresponding expected output. All input must be successfully parsed
        /// </summary>
        /// <typeparam name="T">The type of output</typeparam>
        /// <param name="inputToExpectedOutputMap">The map between the input and the expected output</param>
        /// <param name="parser">The parser.</param>
        public static void TestParser<T>(IDictionary<string, T> inputToExpectedOutputMap, Parser<T> parser)
        {
            foreach (var inputToExpectedOutput in inputToExpectedOutputMap)
            {
                var result = parser.TryParse(inputToExpectedOutput.Key);
                Assert.IsTrue(result.WasSuccessful);
                Assert.AreEqual<T>(inputToExpectedOutput.Value, result.Value);
            }
        }

        /// <summary>
        /// Tests a parse that outputs multiple tokens
        /// </summary>
        /// <typeparam name="T">The type of output</typeparam>
        /// <param name="inputToExpectedOutputMap">The map between the input and the expected output</param>
        /// <param name="parser">The parser.</param>
        public static void TestMultiTokenParse<T>(IDictionary<string, IEnumerable<T>> inputToExpectedOutputMap, Parser<IEnumerable<T>> parser)
        {
            foreach (var kvp in inputToExpectedOutputMap)
            {
                var result = parser.TryParse(kvp.Key);
                Assert.IsTrue(result.WasSuccessful);
                Assert.IsTrue(Enumerable.SequenceEqual<T>(kvp.Value, result.Value));
            }
        }
    }
}