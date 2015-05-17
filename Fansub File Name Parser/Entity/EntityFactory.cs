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
using Functional.Maybe;
using Sprache;
using System;
using System.Collections.Generic;

namespace FansubFileNameParser.Entity
{
    /// <summary>
    /// A static factory class for all <see cref="IFansubEntity"/> objects
    /// </summary>
    public static class EntityFactory
    {
        #region private static fields
        private static readonly ISet<string> MediaFileExtensions = new HashSet<string>
        {
            "AVI",
            "M2TS",
            "MKV",
            "MP4",
            "TS",
            "WMV",
        };
        #endregion

        #region public methods
        /// <summary>
        /// Tries the parse.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <returns></returns>
        public static Maybe<IFansubEntity> TryParse(string inputString)
        {
            var preprocessedString = PreprocessString(inputString);
            var mediaMetadata = MediaMetadataParser.TryParseMediaMetadataWithMaybe(preprocessedString);

            return Maybe<IFansubEntity>.Nothing;
        }
        #endregion
        #region private methods
        #region input string preprocessing
        private static string PreprocessString(string inputString)
        {
            return RemoveDots(RemoveUnderscores(inputString));
        }

        private static string RemoveUnderscores(string inputString)
        {
            return inputString.Replace('_', ' ');
        }

        private static string RemoveDots(string inputString)
        {
            var indexOfLastDot = inputString.LastIndexOf('.');
            if (indexOfLastDot == inputString.Length - 1 || indexOfLastDot == 0)
            {
                /*
                 * The last dot is either the last character or it's the first 
                 * character, which means we're just going to remove it entirely
                 */
                return inputString.Replace('.', ' ');
            }
            else if (indexOfLastDot < 0)
            {
                // There are no dots. Do nothing
                return inputString;
            }
            else
            {
                /*
                 * The last dot is somewhere in the middle of the string. Let's take
                 * the characters after it and remove all the other dots
                 */
                var possibleExtension = inputString.Substring(indexOfLastDot + 1);
                var everythingBeforeLastLastDot = inputString.Substring(0, indexOfLastDot);

                if (MediaFileExtensions.Contains(possibleExtension.ToUpperInvariant()))
                {
                    // This is a media file extension
                    return inputString.Substring(0, indexOfLastDot).Replace('.', ' ') + '.' + possibleExtension;
                }

                /*
                 * The characters after the last dot do not correspond to a file extension. 
                 * Remove all of the dots
                 */

                return inputString.Replace('.', ' ');
            }

        }
        #endregion
        #endregion
    }
}
