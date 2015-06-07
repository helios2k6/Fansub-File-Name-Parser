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

using FansubFileNameParser.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnitTests.Models;

namespace Tests
{
    [TestClass]
    public sealed class MediaMetadataParseTests
    {
        [TestMethod]
        public void ParseAllTags()
        {
            Parallel.ForEach(TestModel.CreateMediaMetadataTestModel().Take(2000), kvp =>
            {
                var inputTags = kvp.Key;
                var expectedMetadata = kvp.Value;
                var experimentalMetadataMaybe = MediaMetadataParser.TryParseMediaMetadata(inputTags);

                if (experimentalMetadataMaybe.HasValue)
                {
                    Assert.AreEqual<MediaMetadata>(expectedMetadata, experimentalMetadataMaybe.Value);
                }
            });
        }

        [TestMethod]
        public void SerializeAndDeserialize()
        {
            Parallel.ForEach(TestModel.CreateMediaMetadataTestModel().Take(2000), kvp =>
            {
                TestSerializationAndDeserialization(kvp.Value);
            });
        }

        private static void TestSerializationAndDeserialization(MediaMetadata originalObject)
        {
            using (var memoryStream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, originalObject);

                memoryStream.Seek(0, SeekOrigin.Begin);

                var deserializedObject = (MediaMetadata)formatter.Deserialize(memoryStream);

                Assert.AreEqual<MediaMetadata>(originalObject, deserializedObject);
            }
        }
    }
}
