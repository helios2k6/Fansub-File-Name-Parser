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

using FansubFileNameParser.Entity.Parsers;
using System;
using System.Collections.Generic;

namespace PerformanceTest
{
    public static class Program
    {
        private static readonly string[] PerformanceTestInput = new string[] 
        {
            "[HorribleSubs] Working!!! - 07 [720p].mkv",
            "[Commie] Teekyuu - 38 [76ADB77A].mkv",
            "[Coalgirls]_Cross_Ange_01-03_(1280x720_Blu-ray_FLAC)",
            "[Commie] Cross Ange - Tenshi to Ryuu no Rondo - Volume 1 [BD 1080p AAC]",
            "[Commie] Ore Monogatari!! - 01 [0C0EE0AB].mkv",
            "[FFF] Ore Monogatari!! - 01 [12223D77].mkv",
            "[HorribleSubs] Ore Monogatari!! - 01 [720p].mkv",
            "[Chyuu-Migoto] Danna ga Nani wo Itteiru ka Wakaranai Ken 2 Sure-me - 04 [720p][6DDB4B9B].mkv",
            "[HorribleSubs] Kekkai Sensen - 01 [720p].mkv",
            "[Commie] Takamiya Nasuno Desu! - 01 [A24A2396].mkv",
            "[HorribleSubs] Takamiya Nasuno Desu! - 01 [720p].mkv",
            "[Underwater] Knights of Sidonia S2 - The Ninth Planet Crusade - 01 (720p) [A631962C].mkv",
            "[Commie] Nisekoi 2 - 01 [4EA59436].mkv",
            "[Commie] Plastic Memories - 01 [D921F939].mkv",
            "[Chyuu] Tokyo Ghoul - Vol.3 (BD 1080p FLAC)",
            "[HorribleSubs] Yahari Ore no Seishun Love Come wa Machigatteiru Zoku - 01 [720p].mkv",
            "[FFF] Nisekoi S2 - 01 [E0D0C713].mkv",
            "[Commie] Fate⁄stay Night Unlimited Blade Works - 13 [E19030CB].mkv",
            "[FFF] Hibike! Euphonium - 01 [A72D5FD8].mkv",
            "[HorribleSubs] Ghost in the Shell Arise - Alternative Architecture - 01 [720p].mkv",
            "[Commie] Teekyuu - 37 [BAC36373].mkv",
            "[HorribleSubs] Hibike! Euphonium - 01 [720p].mkv",
            "[Vivid] Kekkai Sensen - 01 [81AA3BB7].mkv",
            "[HorribleSubs] Teekyu S4 - 37 [720p].mkv",
            "[HorribleSubs] The Disappearance of Nagato Yuki-chan - 01 [720p].mkv",
            "[HorribleSubs] Fate Stay Night - Unlimited Blade Works - 13 [720p].mkv",
            "[HorribleSubs] Gunslinger Stratos - 01 [720p].mkv",
            "[HorribleSubs] Plastic Memories - 01 [720p].mkv",
            "[FFF] Yahari Ore no Seishun Love Come wa Machigatteiru. Zoku - 01 [A5EB9E23].mkv",
            "[Kaitou] Danna ga Nani wo Itteiru ka Wakaranai Ken 2 Sure-me (I Can't Understand What My Husband Is Saying 2nd Thread) - 01 [720p][10bit][811C80FE].mkv",
            "[HorribleSubs] Nisekoi S2 - 00 [720p].mkv",
            "[Commie] Yahari Ore no Seishun Love Comedy wa Machigatteiru. Zoku - My Teenage RomCom SNAFU TOO! - 01 [E3B1108A].mkv",
            "[HorribleSubs] Danna ga Nani wo Itteiru ka Wakaranai Ken S2 - 01 [720p].mkv",
            "[Commie] Psycho-Pass 2 - Volume 4 [BD 1080p FLAC]",
            "[BlurayDesuYo] Amagi Brilliant Park - Vol. 1 (BD 1920x1080 10bit FLAC)",
            "[Coalgirls]_Amagi_Brilliant_Park_03-04_(1920x1080_Blu-Ray_FLAC)",
            "[Doki] Sakura Trick - Vol 4 (1920x1080 Hi10P BD FLAC)",
            "[ReinForce] Hanamonogatari (BDRip 1920x1080 x264 FLAC)",
            "[ReinForce] Tsukimonogatari (BDRip 1920x1080 x264 FLAC)",
            "[Seed-Raws] Tsukimonogatari (BD 1280x720 AVC AAC)",
            "[IEgg] ShiroBako Vol.03 (BD 1920X1080 x264 FLAC)",
            "[ReinForce] Shirobako - Vol.1 (BDRip 1920x1080 x264 FLAC)",
        };

        public static void Main(string[] args)
        {
            foreach (var testInput in PerformanceTestInput)
            {
                var result = EntityParsers.TryParseEntity(testInput);
                if (result.HasValue)
                {
                    var value = result.Value;
                    Console.WriteLine("Parsed {0}", result);
                }
            }
        }
    }
}
