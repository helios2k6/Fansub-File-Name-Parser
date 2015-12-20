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
using System.Collections.Generic;

namespace FansubFileNameParser.Utils
{
    /// <summary>
    /// Utility class for translating a roman numeral to an integer
    /// </summary>
    internal static class RomanNumerals
    {
        #region private fields
        private static readonly IDictionary<char, int> RomanNumeralToValueMap = new Dictionary<char, int>
        {
            {'I', 1},
            {'V', 5},
            {'X', 10},
        };
        #endregion

        #region public methods
        /// <summary>
        /// Translates the roman numeral to an integer
        /// </summary>
        /// <param name="romanNumerals">The roman numeral digit.</param>
        /// <returns>The integer that corresponds to this roman numeral</returns>
        public static int TranslateRomanNumeralToInt(string romanNumerals)
        {
            return TranslateRomanNumeralToIntImpl(romanNumerals);
        }
        #endregion

        #region private methods
        private static bool IsRomanNumeral(char a)
        {
            return a == 'I' || a == 'V' || a == 'X';
        }

        private static int CompareRomanNumerals(char left, char right)
        {
            // Sanity check
            if (IsRomanNumeral(left) == false || IsRomanNumeral(right) == false)
            {
                throw new ArgumentException(string.Format("One of these characters is not a roman numeral. Left = {0} | Right = {1}", left, right));
            }

            if (left == 'I')
            {
                return right == 'I'
                    ? 0
                    : -1;
            }
            else if (left == 'V')
            {
                if (right == 'I')
                {
                    return 1;
                }
                else if (right == 'V')
                {
                    return 0;
                }

                return -1;
            }

            return right == 'X'
                ? 0
                : 1;
        }

        private static int TranslateRomanNumeralToIntImpl(string romanNumerals)
        {
            int runningSum = 0;
            string upperCased = romanNumerals.ToUpperInvariant();
            for (int i = 0; i < upperCased.Length; i++)
            {
                char currentChar = upperCased[i];
                // If there's a char ahead, then we have to check to see if it's bigger than us
                int subtractedAmount = 0;
                if (i + 1 < upperCased.Length)
                {
                    char forwardChar = romanNumerals[i + 1];
                    int compareRomanNumerals = CompareRomanNumerals(currentChar, forwardChar);

                    if (compareRomanNumerals < 0)
                    {
                        subtractedAmount = RomanNumeralToValueMap[forwardChar] - RomanNumeralToValueMap[currentChar];
                    }
                }

                // We need to add the subtracted amount due to the way you can shorten Roman Numerals
                if (subtractedAmount > 0)
                {
                    runningSum += subtractedAmount;
                }
                else
                {
                    runningSum += RomanNumeralToValueMap[currentChar];
                }
            }

            return runningSum;
        }
        #endregion
    }
}
