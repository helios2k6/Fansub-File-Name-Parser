﻿/*
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
            IEnumerable<KeyValuePair<string, IFansubEntity>> model,
            Parser<IFansubEntity> parser
        )
        {
            foreach (var stringToModel in model)
            {
                var fansubString = stringToModel.Key;
                var expectedParseResult = stringToModel.Value;
                var parseResult = parser.TryParse(fansubString);
                Assert.AreEqual<Type>(typeof(TResultType), expectedParseResult.GetType());
                Assert.IsTrue(parseResult.WasSuccessful);
                Assert.AreEqual<Type>(expectedParseResult.GetType(), parseResult.Value.GetType());
                Assert.AreEqual<TResultType>((TResultType)expectedParseResult, (TResultType)parseResult.Value);
            }
        }

        [TestMethod]
        public void TestParseOPED()
        {
            var coreParser = from metadata in BaseEntityParsers.MediaMetadata.OptionalMaybe().ResetInput()
                             from fansubGroup in BaseEntityParsers.FansubGroup.OptionalMaybe().ResetInput()
                             from series in BaseEntityParsers.SeriesName.OptionalMaybe().ResetInput()
                             from ext in FileEntityParsers.FileExtension.OptionalMaybe()
                             where series.HasValue
                             select new
                             {
                                 Metatdata = metadata,
                                 Group = fansubGroup,
                                 Series = OPEDEntityParsers.FilterOutNonCreditsFromSeriesName(series),
                                 Extension = ext,
                             };

            var input = "[Coalgirls]_Valkyria_Chronicles_OP1_-_alt_(1920x1080_Blu-Ray_FLAC)_[414F6192].mkv";
            var parser = from _0 in BaseGrammars.CleanInputString.SetResultAsRemainder()
                         from core in coreParser.ResetInput()
                         //from _1 in BaseGrammars.ContentBetweenTagGroups.SetResultAsRemainder()
                         //from _2 in Parse.String(core.Series.Value).Token().Text()
                         select new
                         {
                             Core = core,
                             //A = _1,
                             //B = _2,
                         };

            var result = parser.TryParse(input);
            Console.WriteLine();
            //TestParserHelper<FansubOPEDEntity>(TestModel.OpeningEndingTestModel, EntityParsers.EntityParser);
        }
    }
}
