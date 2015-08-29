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

namespace FansubFileNameParser
{
    /// <summary>
    /// The basic grammars that power this entire library
    /// </summary>
    internal static class BaseGrammars
    {
        #region Single Characters
        /// <summary>
        /// Parses a single dash ('-') character
        /// </summary>
        public static Parser<char> Dash
        {
            get { return Parse.Char('-'); }
        }

        /// <summary>
        /// Parses a single dot ('.') character
        /// </summary>
        public static Parser<char> Dot
        {
            get { return Parse.Char('.'); }
        }

        /// <summary>
        /// Parses a single underscore ('_') character
        /// </summary>
        public static Parser<char> Underscore
        {
            get { return Parse.Char('_'); }
        }

        /// <summary>
        /// Parses a single open parenthesis character ('(')
        /// </summary>
        public static Parser<char> OpenParenthesis
        {
            get { return Parse.Char('('); }
        }

        /// <summary>
        /// Parses a single closed parenthesis character (')')
        /// </summary>
        public static Parser<char> ClosedParenthesis
        {
            get { return Parse.Char(')'); }
        }

        /// <summary>
        /// Parses a single a open square bracket ('[')
        /// </summary>
        public static Parser<char> OpenSquareBracket
        {
            get { return Parse.Char('['); }
        }

        /// <summary>
        /// Parses a single closed square bracket (']')
        /// </summary>
        public static Parser<char> ClosedSquareBracket
        {
            get { return Parse.Char(']'); }
        }
        #endregion
        #region Single Concepts
        /// <summary>
        /// Parses any open metadata tag deliminator, such as the open parenthesis or open square bracket
        /// </summary>
        public static Parser<char> OpenTagDeliminator
        {
            get { return OpenParenthesis.Or(OpenSquareBracket); }
        }

        /// <summary>
        /// Parses any closed metadata tag deliminator, such as the closed parenthesis or closed square bracket
        /// </summary>
        public static Parser<char> ClosedTagDeliminator
        {
            get { return ClosedParenthesis.Or(ClosedSquareBracket); }
        }

        /// <summary>
        /// Parses a single tag delminator
        /// </summary>
        public static Parser<char> TagDeliminator
        {
            get { return OpenTagDeliminator.Or(ClosedTagDeliminator); }
        }
        #endregion
        #region Multi-character Concepts
        /// <summary>
        /// Parses a string of characters, including whitespace. This always succeedes, even if there are
        /// no characters to parse.
        /// </summary>
        public static Parser<string> Line
        {
            get { return Parse.AnyChar.Many().Text(); }
        }

        /// <summary>
        /// Parses a single dash separator token (' - ')
        /// </summary>
        public static Parser<string> DashSeparatorToken
        {
            get
            {
                return from frontSpace in Parse.WhiteSpace
                       from dash in Dash
                       from backSpace in Parse.WhiteSpace
                       select new string(new[] { frontSpace, dash, backSpace });
            }
        }
        #endregion
    }
}