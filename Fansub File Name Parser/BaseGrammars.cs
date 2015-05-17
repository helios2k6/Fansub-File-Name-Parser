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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace FansubFileNameParser
{
    /// <summary>
    /// The basic grammars that power this entire library
    /// </summary>
    internal static class BaseGrammars
    {
        #region basic parsers
        /// <summary>
        /// Parses an entire line of text, including special characters
        /// </summary>
        public static readonly Parser<string> Line = Parse.AnyChar.AtLeastOnce().Text();
        /// <summary>
        /// Parses a whole word, which is a contiguous set of letters with at least 1 letter in it
        /// </summary>
        public static readonly Parser<string> Identifier = Parse.Letter.AtLeastOnce().Text().Token();
        /// <summary>
        /// Parses a single underscore ('_') character
        /// </summary>
        public static readonly Parser<string> Underscore = Parse.Char('_').AtLeastOnce().Text();
        /// <summary>
        /// Parses a single dash ('-') character
        /// </summary>
        public static readonly Parser<string> Dash = Parse.Char('-').AtLeastOnce().Text();
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
        public static readonly Parser<char> OpenTagDeliminator = OpenParenthesis.Or(OpenSquareBracket);
        /// <summary>
        /// Parses any closed metadata tag deliminator, such as the closed parenthesis or closed square bracket
        /// </summary>
        public static readonly Parser<char> ClosedTagDeliminator = ClosedParenthesis.Or(ClosedSquareBracket);
        /// <summary>
        /// Parses a single tag delminator
        /// </summary>
        private static readonly Parser<char> TagDeliminator = OpenTagDeliminator.Or(ClosedTagDeliminator);
        #endregion
        #region line parsers
        /// <summary>
        /// Parses a line of text until a dash character is hit
        /// </summary>
        private static readonly Parser<string> LineUntilDash = Parse.AnyChar.Until(Dash).Text();
        /// <summary>
        /// Parses a line of text until a dash character or a full line if there wasn't a dash character
        /// </summary>
        private static readonly Parser<string> LineUntilDashOrFullLine = LineUntilDash.Or(Line);
        /// <summary>
        /// Parses a line of text until a tag delmiinator is encountered
        /// </summary>
        public static readonly Parser<string> LineUntilTagDeliminator = Parse.AnyChar.Until(TagDeliminator).Text();
        #endregion
        #region bracket parsers
        /// <summary>
        /// Parses a single metadata tag
        /// </summary>
        public static readonly Parser<string> TagEnclosedText =
            (from openTag in OpenTagDeliminator
             from content in LineUntilTagDeliminator
             from closedBracket in ClosedTagDeliminator
             select content).Token();

        /// <summary>
        /// Parses a single metadata tag, but includes the metadata tag deliminator
        /// </summary>
        public static readonly Parser<string> TagEnclosedTextWithDeliminator =
            (from openTag in OpenTagDeliminator
             from content in LineUntilTagDeliminator
             from closedBracket in ClosedTagDeliminator
             select string.Concat(openTag, content, closedBracket)).Token();

        #endregion
        #region lexers
        #region lexers by line
        /// <summary>
        /// Parses a stream of lines separated by a dash
        /// </summary>
        public static readonly Parser<IEnumerable<string>> LinesSeparatedByDash = LineUntilDashOrFullLine.Many();
        #endregion
        #region lexers by tag
        /// <summary>
        /// Parses multiple metadata tags
        /// </summary>
        public static readonly Parser<IEnumerable<string>> MultipleTagEnclosedText = TagEnclosedText.Many();
        #endregion
        #endregion
    }
}