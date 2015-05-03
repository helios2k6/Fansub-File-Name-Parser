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

using FansubFileNameParser.Entity;
using Functional.Maybe;
using Sprache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FansubFileNameParser
{
    /// <summary>
    /// A factory class for producing <seealso cref="Entity.IFansubEntity"/>'s given a string
    /// </summary>
    public static class EntityParsers
    {
        #region private classes and enums
        private enum DetectedEntity
        {
            Directory,
            File,
        }

        private sealed class ParseState
        {
            public string OriginalInput { get;set; }
            public Maybe<string> MainContent { get; set; }
            public DetectedEntity EntityType { get; set; }
            public Maybe<string> Extension { get; set; }
            public Maybe<string> Group { get; set; }
            public IEnumerable<string> Tags { get; set; }
        }
        #endregion

        #region private static fields
        private static readonly ISet<string> MediaFileExtensions = new HashSet<string>
        {
            "AVI",
            "MKV",
            "MP4",
            "M2TS",
            "OGM",
            "TS",
            "WEBM",
            "WMV",
        };
        #endregion
        /// <summary>
        /// Tries to parse the string into an <seealso cref="IFansubEntity"/> given the string
        /// </summary>
        /// <returns>True if an <seealso cref="IFansubEntity"/> was successfully parsed. False otherwise</returns>
        /// <param name="input">The string input to parse.</param>
        public static Maybe<IFansubEntity> TryParse(string input)
        {
            // Check for invalid file names, which, in this case, would be an empty string
            if (string.IsNullOrWhiteSpace(input))
            {
                return Maybe<IFansubEntity>.Nothing;
            }

            // Create the initial parse state
            var parseState = new ParseState 
            {
                OriginalInput = input,
            };
            
            DetectEntityType(parseState);


            return Maybe<IFansubEntity>.Nothing;
        }

        #region private static functions
        #region stage 1 - determine if directory or file
        private static void DetectEntityType(ParseState state)
        {
            var extension = Path.GetExtension(state.OriginalInput).ToUpperInvariant();
            if (MediaFileExtensions.Contains(extension))
            {
                state.EntityType = DetectedEntity.File;
            }
            else
            {
                state.EntityType = DetectedEntity.Directory;
            }
        }
        #endregion
        #region stage 2 - break apart string
        private static bool TryBreakApartString(ParseState state)
        {
            var separatedContentResults = BaseParsers.SeparateTagsFromMainContent.TryParse(state.OriginalInput);
            if(separatedContentResults.WasSuccessful)
            {
                var value = separatedContentResults.Value;

                state.Group = value.Group;
                state.MainContent = value.Content;
                state.Tags = value.Tags;

                return true;
            }

            return false;
        }
        #endregion
        #region stage 3a - process directory
        private static Maybe<IFansubEntity> TryParseDirectoryEntity(ParseState state)
        {
            return Maybe<IFansubEntity>.Nothing;
        }
        #endregion
        #region stage 3b - process single file
        private static Maybe<IFansubEntity> TryParseFileEntity(ParseState state)
        {
            return Maybe<IFansubEntity>.Nothing;
        }
        #endregion
        #endregion
    }
}

