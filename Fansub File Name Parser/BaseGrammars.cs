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

using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FansubFileNameParser
{
    /// <summary>
    /// The basic grammars that power this entire library
    /// </summary>
    internal static class BaseGrammars
    {
        /// <summary>
        /// Parses a single dash ('-') character
        /// </summary>
        public static readonly Parser<char> Dash = Parse.Char('-');

        /// <summary>
        /// Parses a single underscore ('_') character
        /// </summary>
        private static readonly Parser<char> Underscore = Parse.Char('_');

        /// <summary>
        /// Parses a single dash separator token (' - ')
        /// </summary>
        private static readonly Parser<string> DashSeparatorToken =
            from frontSpace in Parse.WhiteSpace
            from dash in BaseGrammars.Dash
            from backSpace in Parse.WhiteSpace
            select new string(new[] { frontSpace, dash, backSpace });

        /// <summary>
        /// Parses a single open parenthesis character ('(')
        /// </summary>
        private static readonly Parser<char> OpenParenthesis = Parse.Char('(');

        /// <summary>
        /// Parses a single closed parenthesis character (')')
        /// </summary>
        private static readonly Parser<char> ClosedParenthesis = Parse.Char(')');

        /// <summary>
        /// Parses a single a open square bracket ('[')
        /// </summary>
        private static readonly Parser<char> OpenSquareBracket = Parse.Char('[');

        /// <summary>
        /// Parses a single closed square bracket (']')
        /// </summary>
        private static readonly Parser<char> ClosedSquareBracket = Parse.Char(']');

        /// <summary>
        /// Parses any open metadata tag deliminator, such as the open parenthesis or open square bracket
        /// </summary>
        private static readonly Parser<char> OpenTagDeliminator = OpenParenthesis.Or(OpenSquareBracket);

        /// <summary>
        /// Parses any closed metadata tag deliminator, such as the closed parenthesis or closed square bracket
        /// </summary>
        private static readonly Parser<char> ClosedTagDeliminator = ClosedParenthesis.Or(ClosedSquareBracket);

        /// <summary>
        /// Parses a single tag delminator
        /// </summary>
        private static readonly Parser<char> TagDeliminator = OpenTagDeliminator.Or(ClosedTagDeliminator);

        /// <summary>
        /// Parses a string of text up until a "dash separator token," which is defined as a dash (-) with a 
        /// single space before and after it: (" - ")
        /// </summary>
        public static readonly Parser<string> LineUpToDashSeparatorToken =
            from line in Parse.AnyChar.Except(DashSeparatorToken).Many().Text()
            select line.Trim();

        /// <summary>
        /// Parses a line of text until a tag delmiinator is encountered
        /// </summary>
        public static readonly Parser<string> LineUpToTagDeliminator =
            from line in Parse.AnyChar.Except(TagDeliminator).Many().Text()
            select line.Trim();

        /// <summary>
        /// Parses the content of a metatag
        /// </summary>
        public static readonly Parser<string> MetaTagContent = 
            LineUpToTagDeliminator.Contained(OpenTagDeliminator, ClosedTagDeliminator);

        /// <summary>
        /// Parses a single metadata tag, but includes the metadata tag deliminator
        /// </summary>
        public static readonly Parser<string> MetaTag =
            from openTag in OpenTagDeliminator
            from content in LineUpToTagDeliminator
            from closedBracket in ClosedTagDeliminator
            select string.Concat(openTag, content, closedBracket);

        /// <summary>
        /// Parses multiple metadata tags
        /// </summary>
        public static readonly Parser<IEnumerable<string>> MetaTagGroup = MetaTagContent.Token().Many();

        /// <summary>
        /// Parses content that's contained between any group of multiple tags
        /// </summary>
        public static readonly Parser<string> ContentBetweenTagGroups =
            LineUpToTagDeliminator.Contained(MetaTagGroup.Optional(), MetaTagGroup.Optional());

        /// <summary>
        /// Parses and captures all of the metadata tags that appear in the string, including the tag deliminator token
        /// </summary>
        public static readonly Parser<IEnumerable<string>> CollectTags =
            from tags in
                (from tag in MetaTagGroup.Optional()
                 from _ in Parse.AnyChar.Except(MetaTag).Optional()
                 select tag.ToIEnumerable()).Many().Implode()
            select tags;

        /// <summary>
        /// Parses any media file extension
        /// </summary>
        private static readonly Parser<string> MediaFileExtension =
            Parse.IgnoreCase(".AVI")
            .Or(Parse.IgnoreCase(".MKV"))
            .Or(Parse.IgnoreCase(".MP4"))
            .Or(Parse.IgnoreCase(".M2TS"))
            .Or(Parse.IgnoreCase(".OGM"))
            .Or(Parse.IgnoreCase(".TS"))
            .Or(Parse.IgnoreCase(".WMV"))
            .End()
            .Text();

        /// <summary>
        /// Replaces the dots in the string with spaces while preserving the file extension of a media file
        /// TODO: WRITE UNIT TESTS
        /// </summary>
        public static readonly Parser<string> ReplaceDotsExceptMediaFileExtension =
            from frontSegment in Parse.AnyChar.Except(MediaFileExtension).Many().Text()
            from extension in MediaFileExtension.Optional()
            select string.Format(
                "{0}{1}", 
                frontSegment.Replace('.', ' '), 
                extension.IsDefined 
                ? extension.Get() 
                : string.Empty
            );

        /// <summary>
        /// Replaces the underscores of an input string with spaces
        /// TODO: WRITE UNIT TESTS
        /// </summary>
        public static readonly Parser<string> ReplaceUnderscores =
            from segments in
                (from content in Parse.AnyChar.Except(Underscore).Many().Text()
                 from _ in Underscore
                 select content
                ).Many().Implode(string.Empty, (acc, i) => string.Format("{0} {1}", acc, i))
            select segments;
    }
}