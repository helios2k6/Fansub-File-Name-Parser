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
        public static readonly Parser<string> OpenParenthesis = Parse.Char('(').Once().Text();
        /// <summary>
        /// Parses a single closed parenthesis character (')')
        /// </summary>
        public static readonly Parser<string> ClosedParenthesis = Parse.Char(')').Once().Text();
        /// <summary>
        /// Parses a single a open square bracket ('[')
        /// </summary>
        public static readonly Parser<string> OpenSquareBracket = Parse.Char('[').Once().Text();
        /// <summary>
        /// Parses a single closed square bracket (']')
        /// </summary>
        public static readonly Parser<string> ClosedSquareBracket = Parse.Char(']').Once().Text();
        #endregion
        #region identifier parsers
        /// <summary>
        /// Parses an identifier until an underscore character is hit
        /// </summary>
        public static readonly Parser<string> IdentifierUntilUnderscore = Parse.Letter.Until(Underscore).Text();
        /// <summary>
        /// Parses an identifier until an underscore or space character is hit
        /// </summary>
        public static readonly Parser<string> IdentifierUntilUnderscoreOrFullWord = IdentifierUntilUnderscore.Or(Identifier);
        /// <summary>
        /// Parses an identifier until a dash character is hit
        /// </summary>
        public static readonly Parser<string> IdentifierUntilDash = Parse.Letter.Until(Dash).Text();
        /// <summary>
        /// Parses an identifier until a dash or space character is hit
        /// </summary>
        public static readonly Parser<string> IdentifierUntilDashOrFullWord = IdentifierUntilDash.Or(Identifier);
        #endregion
        #region bracket parsers
        /// <summary>
        /// Parse the contents of a square bracket tag
        /// </summary>
        public static readonly Parser<string> SquareBracketEnclosedText =
            (from openBracket in OpenSquareBracket
             from content in Parse.CharExcept(']').Many().Text()
             from closedBracket in ClosedSquareBracket
             select content).Token();
        
        /// <summary>
        /// Parse a square bracket tag, returning the contents of the tag including the square brackets
        /// </summary>
        public static readonly Parser<string> SquareBracketEnclosedTextWithBracket =
            (from openBracket in OpenSquareBracket
             from content in Parse.CharExcept(']').Many().Text()
             from closedBracket in ClosedSquareBracket
             select string.Concat(openBracket, content, closedBracket)).Token();

        /// <summary>
        /// Parses the contents of a parenthesis tag
        /// </summary>
        public static readonly Parser<string> ParenthesisEnclosedText =
            (from openParenthesis in OpenParenthesis
             from content in Parse.CharExcept(')').Many().Text()
             from closedParenthesis in ClosedParenthesis
             select content).Token();

        /// <summary>
        /// Parses a parenthesis tag, returning the contents of the tag including the parenthesis
        /// </summary>
        public static readonly Parser<string> ParenthesisEnclosedTextWithParenthesis =
            (from openParenthesis in OpenParenthesis
             from content in Parse.CharExcept(')').Many().Text()
             from closedParenthesis in ClosedParenthesis
             select string.Concat(openParenthesis, content, closedParenthesis)).Token();
        #endregion
        #region line parsers
        /// <summary>
        /// Parses a line of text until a dash character is hit
        /// </summary>
        public static readonly Parser<string> LineUntilDash = Parse.AnyChar.Until(Dash).Text();
        /// <summary>
        /// Parses a line of text until a dash character or a full line if there wasn't a dash character
        /// </summary>
        public static readonly Parser<string> LineUntilDashOrFullLine = LineUntilDash.Or(Line);
        /// <summary>
        /// Parses a line of text until an underscore character is hit
        /// </summary>
        public static readonly Parser<string> LineUntilUnderscore = Parse.AnyChar.Until(Underscore).Text();
        /// <summary>
        /// Parses a line of text until an underscore character is hit or the full line if there wasn't an underscore character
        /// </summary>
        public static readonly Parser<string> LineUntilUnderscoreOrFullLine = LineUntilUnderscore.Or(Line);
        /// <summary>
        /// Parses a line of text until an open square bracket is hit
        /// </summary>
        public static readonly Parser<string> LineUntilOpenSquareBracket = Parse.AnyChar.Until(OpenSquareBracket).Text();
        /// <summary>
        /// Parses a line of text until an open parenthesis is hit
        /// </summary>
        public static readonly Parser<string> LineUntilOpenParenthesis = Parse.AnyChar.Until(OpenParenthesis).Text();
        /// <summary>
        /// Parses a line of text until a square bracket or an open parenthesis is hit
        /// </summary>
        public static readonly Parser<string> LineUntilSquareBracketOrParenthesis = LineUntilOpenSquareBracket.Or(LineUntilOpenParenthesis).Text();
        /// <summary>
        /// Parses a line of text until a numerical digit is hit
        /// </summary>
        public static readonly Parser<string> LineUntilDigit = Parse.AnyChar.Until(Parse.Number).Text();
        /// <summary>
        /// Parses a line until a numerical digit is hit or the entire line if no digit was hit
        /// </summary>
        public static readonly Parser<string> LineUntilDigitOrFullLine = LineUntilDigit.Or(Line);
        #endregion
        #region lexers
        #region lexers by identifiers
        /// <summary>
        /// Parses a stream of identifiers that are separated by an underscore
        /// </summary>
        public static readonly Parser<IEnumerable<string>> IdentifiersSeparatedByUnderscore = IdentifierUntilUnderscoreOrFullWord.Many();
        /// <summary>
        /// Parses a stream of identifiers that are separated by a dash
        /// </summary>
        public static readonly Parser<IEnumerable<string>> IdentifiersSeparatedByDash = IdentifierUntilDashOrFullWord.Many();
        #endregion
        #region lexers by line
        /// <summary>
        /// Parses a stream of lines separated by an underscore
        /// </summary>
        public static readonly Parser<IEnumerable<string>> LinesSeparatedByUnderscore = LineUntilUnderscoreOrFullLine.Many();
        /// <summary>
        /// Parses a stream of lines separated by a dash
        /// </summary>
        public static readonly Parser<IEnumerable<string>> LinesSeparatedByDash = LineUntilDashOrFullLine.Many();
        #endregion
        #region lexers by brackets
        /// <summary>
        /// Parses a stream of square bracket tags or parenthesis tags
        /// </summary>
        public static readonly Parser<IEnumerable<string>> TagLexerWithBrackets =
            SquareBracketEnclosedTextWithBracket.Or(ParenthesisEnclosedTextWithParenthesis).Many();
        #endregion
        #endregion
    }
}