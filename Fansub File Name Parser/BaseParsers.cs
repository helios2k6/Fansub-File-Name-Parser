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
        internal sealed class SeparatedParseResult : Tuple<Maybe<string>, Maybe<string>, IEnumerable<string>>, IEquatable<SeparatedParseResult>
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

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String" /> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                return string.Format("{0} {1} {2}", Group, Content, Tags);
            }

            /// <summary>
            /// Indicates whether the current object is equal to another object of the same type.
            /// </summary>
            /// <param name="other">An object to compare with this object.</param>
            /// <returns>
            /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
            /// </returns>
            public bool Equals(SeparatedParseResult other)
            {
                if (EqualsPreamble(other) == false)
                {
                    return false;
                }

                return Group.Equals(other.Group)
                    && Content.Equals(other.Content)
                    && Enumerable.SequenceEqual(Tags, other.Tags);
            }

            /// <summary>
            /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
            /// </summary>
            /// <param name="other">The <see cref="System.Object" /> to compare with this instance.</param>
            /// <returns>
            ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
            /// </returns>
            public override bool Equals(object other)
            {
                return Equals(other as SeparatedParseResult);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public override int GetHashCode()
            {
                return Group.GetHashCode()
                    ^ Content.GetHashCode()
                    ^ Tags.Sum(t => t.GetHashCode());
            }

            private bool EqualsPreamble(object other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                if (GetType() != other.GetType()) return false;

                return true;
            }
        }
        #endregion
        #region parsers
        /// <summary>
        /// Parses and captures all of the metadata tags (square or parenthesis) that appear in the string. This
        /// includes tags that might appear at the beginning, the middle, or the end of the string.
        /// </summary>
        public static Parser<IEnumerable<string>> AllTags =
            from forwardContent in BaseGrammars.LineExceptTagDeliminator.Optional()
            from frontTags in BaseGrammars.MultipleTagEnclosedText.Optional()
            from centerContent in BaseGrammars.LineExceptTagDeliminator.Optional()
            from centerTags in BaseGrammars.MultipleTagEnclosedText.Optional()
            from endContent in BaseGrammars.LineExceptTagDeliminator.Optional()
            from endingTags in BaseGrammars.MultipleTagEnclosedText.Optional()
            select OptionalConcatOrEmpty(frontTags, centerTags, endingTags);

        /// <summary>
        /// Separates the content within the name of a fansub file or directory by separating the input into three 
        /// sections:
        /// [fansub group] [content] [tags]
        /// </summary>
        public static Parser<SeparatedParseResult> SeparateTagsFromMainContent =
            from fansubGroup in BaseGrammars.TagEnclosedText.Token().Optional()
            from content in BaseGrammars.LineExceptTagDeliminator.Token().Optional()
            from tags in BaseGrammars.MultipleTagEnclosedText.Token()
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

