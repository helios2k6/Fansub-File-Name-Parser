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

using Functional.Maybe;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FansubFileNameParser
{
    /// <summary>
    /// Static class of complex parsers designed to be reused and tested against fansub file or directory strings
    /// </summary>
    internal static class BaseParsers
    {
        #region nested classes
        /// <summary>
        /// A formalized three-tuple that represents the parse results of the <see cref="BaseParsers.SeparateTagsFromMainContent"/> parser
        /// </summary>
        internal sealed class SeparatedParseResult : Tuple<Maybe<string>, Maybe<string>, IEnumerable<string>>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SeparatedParseResult"/> class.
            /// </summary>
            public SeparatedParseResult()
                : base(Maybe<string>.Nothing, Maybe<string>.Nothing, Enumerable.Empty<string>())
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SeparatedParseResult"/> class.
            /// </summary>
            /// <param name="group">The fansub group.</param>
            /// <param name="content">The main content.</param>
            /// <param name="tags">The media metadata tags.</param>
            public SeparatedParseResult(Maybe<string> group, Maybe<string> content, IEnumerable<string> tags)
                : base(group, content, tags)
            {
            }

            /// <summary>
            /// Gets the fansub group.
            /// </summary>
            /// <value>
            /// The fansub group.
            /// </value>
            public Maybe<string> Group
            {
                get { return Item1; }
            }

            /// <summary>
            /// Gets the main content.
            /// </summary>
            /// <value>
            /// The main content.
            /// </value>
            public Maybe<string> Content
            {
                get { return Item2; }
            }

            /// <summary>
            /// Gets the media metadata tags.
            /// </summary>
            /// <value>
            /// The media metadata tags.
            /// </value>
            public IEnumerable<string> Tags
            {
                get { return Item3; }
            }
        }
        #endregion

        #region parsers
        /// <summary>
        /// Parses and captures all of the metadata tags (square or parenthesis) that appear in the string. This
        /// includes tags that might appear at the beginning, the middle, or the end of the string.
        /// </summary>
        public static Parser<IEnumerable<string>> AllTags =
            from forwardContent in BaseGrammars.LineUntilTagDeliminator.Optional()
            from frontTags in BaseGrammars.MultipleTagEnclosedText.Optional()
            from centerContent in BaseGrammars.LineUntilTagDeliminator.Optional()
            from centerTags in BaseGrammars.MultipleTagEnclosedText.Optional()
            from endContent in BaseGrammars.LineUntilTagDeliminator.Optional()
            from endingTags in BaseGrammars.MultipleTagEnclosedText.Optional()
            select OptionalConcatOrEmpty(frontTags, centerTags, endingTags);

        /// <summary>
        /// Separates the content within the name of a fansub file or directory by separating the input into three 
        /// sections:
        /// [fansub group] [content] [tags]
        /// </summary>
        public static Parser<SeparatedParseResult> SeparateTagsFromMainContent =
            from fansubGroup in BaseGrammars.TagEnclosedText.Optional()
            from content in Parse.CharExcept(c => c == '[' || c == '(', "Brackets").Many().Text().Optional()
            from tags in BaseGrammars.MultipleTagEnclosedText
            select new SeparatedParseResult(
                fansubGroup.ConvertFromIOptionToMaybe(),
                content.ConvertFromIOptionToMaybe(),
                tags
            );
        #endregion
        #region private methods
        private static IEnumerable<string> OptionalConcatOrEmpty(params IOption<IEnumerable<string>>[] optionalStrings)
        {
            var runningEnumerable = Enumerable.Empty<string>();
            foreach (var optionalString in optionalStrings)
            {
                if (optionalString.IsDefined)
                {
                    runningEnumerable = runningEnumerable.Concat(optionalString.Get());
                }
            }
            return runningEnumerable;
        }
        #endregion
        #region public extension methods
        /// <summary>
        /// Gets the Tags portion that was parsed out of the <see cref="SeparateTagsFromMainContent"/> parser call. This
        /// is mainly used for legacy components that were using the old (deleted) BaseGrammar parser
        /// </summary>
        /// <returns>The tags.</returns>
        /// <param name="this">The instance of the <seealso cref="IResult{SeparatedParseResult}"/>.</param>
        public static IEnumerable<string> GetTags(this IResult<SeparatedParseResult> @this)
        {
            if (@this != null && @this.WasSuccessful)
            {
                return @this.Value.Tags;
            }

            return Enumerable.Empty<string>();
        }
        #endregion
    }
}

