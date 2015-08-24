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
    /// The basic grammars that power this entire library
    /// </summary>
    internal static class BaseGrammars
    {
        /// <summary>
        /// Parses a single dash ('-') character
        /// </summary>
        public static readonly Parser<char> Dash = Parse.Char('-');

        /// <summary>
        /// Parses a single dot ('.') character
        /// </summary>
        public static readonly Parser<char> Dot = Parse.Char('.');

        /// <summary>
        /// Parses a single underscore ('_') character
        /// </summary>
        public static readonly Parser<char> Underscore = Parse.Char('_');

        /// <summary>
        /// Parses a single dash separator token (' - ')
        /// </summary>
        public static readonly Parser<string> DashSeparatorToken =
            from frontSpace in Parse.WhiteSpace
            from dash in BaseGrammars.Dash
            from backSpace in Parse.WhiteSpace
            select new string(new[] { frontSpace, dash, backSpace });

        /// <summary>
        /// Parses a single open parenthesis character ('(')
        /// </summary>
        public static readonly Parser<char> OpenParenthesis = Parse.Char('(');

        /// <summary>
        /// Parses a single closed parenthesis character (')')
        /// </summary>
        public static readonly Parser<char> ClosedParenthesis = Parse.Char(')');

        /// <summary>
        /// Parses a single a open square bracket ('[')
        /// </summary>
        public static readonly Parser<char> OpenSquareBracket = Parse.Char('[');

        /// <summary>
        /// Parses a single closed square bracket (']')
        /// </summary>
        public static readonly Parser<char> ClosedSquareBracket = Parse.Char(']');

        /// <summary>
        /// Parses any open metadata tag deliminator, such as the open parenthesis or open square bracket
        /// </summary>
        public static readonly Parser<char> OpenTagDeliminator = OpenParenthesis.Or(OpenSquareBracket);

        /// <summary>
        /// Parses any closed metadata tag deliminator, such as the closed parenthesis or closed square bracket
        /// </summary>
        public static readonly Parser<char> ClosedTagDeliminator = ClosedParenthesis.Or(ClosedSquareBracket);

        /// <summary>
        /// Parses a single tag delminator
        /// </summary>
        public static readonly Parser<char> TagDeliminator = OpenTagDeliminator.Or(ClosedTagDeliminator);

        /// <summary>
        /// Parses a string of characters, including whitespace. This always succeedes, even if there are
        /// no characters to parse.
        /// </summary>
        public static readonly Parser<string> Line = Parse.AnyChar.Many().Text();

        /// <summary>
        /// Parses a line of text until a tag delmiinator is encountered
        /// </summary>
        public static readonly Parser<string> LineUpToTagDeliminator = ExtraParsers.LineUpTo(TagDeliminator);

        /// <summary>
        /// Parses a version number token
        /// </summary>
        public static readonly Parser<int> VersionNumber =
            from _1 in Parse.Char('v').Or(Parse.Char('V'))
            from number in ExtraParsers.Int
            select number;

        /// <summary>
        /// Parses an episode with an optional version number on it
        /// </summary>
        public static readonly Parser<Tuple<int, Maybe<int>>> EpisodeWithVersionNumber =
            from episodeNumber in ExtraParsers.Int
            from versionNumber in VersionNumber.OptionalMaybe()
            select Tuple.Create(episodeNumber, versionNumber);

        /// <summary>
        /// Parses a episode token of the form {whitespace}{Episode and Version}
        /// </summary>
        public static readonly Parser<Tuple<int, Maybe<int>>> EpisodeVersionWithSpaceToken =
            from _1 in Parse.WhiteSpace
            from episodeAndVersion in EpisodeWithVersionNumber
            select episodeAndVersion;

        /// <summary>
        /// Parses the content of a metatag
        /// </summary>
        public static readonly Parser<string> MetaTagContent =
             BaseGrammars.LineUpToTagDeliminator.Contained(BaseGrammars.OpenTagDeliminator, BaseGrammars.ClosedTagDeliminator);

        /// <summary>
        /// Parses multiple metadata tags
        /// </summary>
        public static readonly Parser<IEnumerable<string>> MetaTagGroup =
            from firstTag in MetaTagContent.AsMany()
            from remainingTags in MetaTagContent.Token().Many()
            select firstTag.Concat(remainingTags);

        /// <summary>
        /// Parses content that's contained between any group of multiple tags
        /// </summary>
        private static readonly Parser<string> ContentBetweenTagGroups =
            LineUpToTagDeliminator.Contained(MetaTagGroup, MetaTagGroup);

        /// <summary>
        /// Parses the main content of a title, which excludes the metatags
        /// </summary>
        public static readonly Parser<string> MainContent =
            ContentBetweenTagGroups.Or(LineUpToTagDeliminator);

        /// <summary>
        /// Parses and captures all of the metadata tags that appear in the string, including the tag deliminator token
        /// </summary>
        public static readonly Parser<IEnumerable<string>> CollectTags =
            ExtraParsers.ScanFor(MetaTagContent).Many();

        /// <summary>
        /// Parses a file extension
        /// </summary>
        public static readonly Parser<string> FileExtension =
            from dot in Dot
            from extContent in Parse.AnyChar.Except(Dot.Or(TagDeliminator)).Many().End().Text()
            select string.Concat(dot, extContent);

        /// <summary>
        /// Replaces the dots in the string with spaces while preserving the file extension of a media file
        /// </summary>
        public static readonly Parser<string> ReplaceDotsExceptMediaFileExtension =
            from frontSegment in Parse.AnyChar.Except(FileExtension).Many().Text()
            from extension in FileExtension.Optional()
            select string.Format(
                "{0}{1}",
                frontSegment.Replace('.', ' '),
                extension.IsDefined
                ? extension.Get()
                : string.Empty
            );

        /// <summary>
        /// Replaces the underscores of an input string with spaces
        /// </summary>
        public static readonly Parser<string> ReplaceUnderscores = ExtraParsers.Filter(Underscore);

        /// <summary>
        /// Sanitizes the input string by replacing dots and dashes with spaces, with the exception 
        /// of any file extensions
        /// </summary>
        public static readonly Parser<string> CleanInputString =
            ReplaceDotsExceptMediaFileExtension.ContinueWith(ReplaceUnderscores);
    }
}