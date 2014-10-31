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

namespace UnitTests.Model.Grammars
{
	[TestClass]
	public class FansubFileParsersTests
	{
		private static readonly IDictionary<string, FansubFile> InputOutputMap = new Dictionary<string, FansubFile>
		{
			{"[Aho-Taku] Sakurasou no Pet na Kanojo - 18 [720p-Hi10P][1D8F695D].mkv", new FansubFile("Aho-Taku", "Sakurasou no Pet na Kanojo", 18, ".mkv")},
			{"[Mazui]_Boku_Ha_Tomodachi_Ga_Sukunai_NEXT_-_05_[12F80420].mkv", new FansubFile("Mazui", "Boku Ha Tomodachi Ga Sukunai NEXT", 5, ".mkv")},
			{"[Anime-Koi] GJ-bu - 05 [h264-720p][E533CA00].mkv", new FansubFile("Anime-Koi", "GJ-bu", 5, ".mkv")},
			{"[WhyNot] Mayo Chiki - 10 [D1DA2637].mkv", new FansubFile("WhyNot", "Mayo Chiki", 10, ".mkv")},
			{"[HorribleSubs] Boku no Imouto wa Osaka Okan - 01 [720p].mkv", new FansubFile("HorribleSubs", "Boku no Imouto wa Osaka Okan", 1, ".mkv")},
			{"[Commie] Ore no Kanojo to Osananajimi ga Shuraba Sugiru - My Girlfriend and Childhood Friend Fight Too Much - 02 [F5ECCCC2].mkv",
				new FansubFile("Commie", "Ore no Kanojo to Osananajimi ga Shuraba Sugiru - My Girlfriend and Childhood Friend Fight Too Much", 2, ".mkv")},
			{"[Doki] Onii-chan Dakedo Ai Sae Areba Kankeinai yo ne - 01 (1280x720 Hi10P AAC) [B66EEF09].mkv", 
				new FansubFile("Doki", "Onii-chan Dakedo Ai Sae Areba Kankeinai yo ne", 1, ".mkv")},
			{"[FFF] Highschool DxD - SP01 [BD][1080p-FLAC][5D929653].mkv", new FansubFile("FFF", "Highschool DxD - SP01", int.MinValue, ".mkv")},
			{"[Eveyuu] Sankarea 00 [DVD Hi10P 480p H264] [4219AF02].mkv", new FansubFile("Eveyuu", "Sankarea", 0, ".mkv")},
			{"[gg]_Sasami-san@Ganbaranai_-_05_[6C2060E1].mkv", new FansubFile("gg", "Sasami-san@Ganbaranai", 5, ".mkv")},
			{"[RaX]Strawberry_Panic_-_01_[No_Dub]_(x264_ogg)_[F4EAA441].mkv", new FansubFile("RaX", "Strawberry Panic", 1, ".mkv")},
			{"(B-A)Devilman_Lady_-_01_(2E088B82).mkv", new FansubFile("B-A", "Devilman Lady", 1, ".mkv")},
			{"[Anime-Koi] GJ-bu - 06v2 [h264-720p][DAC4ACFA].mkv", new FansubFile("Anime-Koi", "GJ-bu", 6, ".mkv")},
			{"[Lunar] Bleach - 05 v2 [F2C9454F].avi", new FansubFile("Lunar", "Bleach", 5, ".avi")}
		};

		[TestMethod]
		public void TryParseFansubFile()
		{
			foreach (var k in InputOutputMap)
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
