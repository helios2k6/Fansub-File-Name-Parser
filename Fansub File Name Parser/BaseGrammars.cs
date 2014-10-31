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

[assembly: InternalsVisibleTo("Tests")]
[assembly: CLSCompliant(false)]
namespace FansubFileNameParser
{
	internal static class BaseGrammars
	{
		#region basic parsers
		public static readonly Parser<string> Line = Parse.AnyChar.AtLeastOnce().Text();
		public static readonly Parser<string> Identifier = Parse.Letter.AtLeastOnce().Text().Token();
		public static readonly Parser<string> Underscore = Parse.Char('_').AtLeastOnce().Text();
		public static readonly Parser<string> Dash = Parse.Char('-').AtLeastOnce().Text();
		public static readonly Parser<string> OpenParenthesis = Parse.Char('(').Once().Text();
		public static readonly Parser<string> ClosedParenthesis = Parse.Char(')').Once().Text();
		public static readonly Parser<string> OpenSquareBracket = Parse.Char('[').Once().Text();
		public static readonly Parser<string> ClosedSquareBracket = Parse.Char(']').Once().Text();
		#endregion
		#region identifier parsers
		public static readonly Parser<string> IdentifierUntilUnderscore = Parse.Letter.Until(Underscore).Text();
		public static readonly Parser<string> IdentifierUntilUnderscoreOrFullWord = IdentifierUntilUnderscore.Or(Identifier);

		public static readonly Parser<string> IdentifierUntilDash = Parse.Letter.Until(Dash).Text();
		public static readonly Parser<string> IdentifierUntilDashOrFullWord = IdentifierUntilDash.Or(Identifier);
		#endregion
		#region line parsers
		public static readonly Parser<string> LineUntilDash = Parse.AnyChar.Until(Dash).Text();
		public static readonly Parser<string> LineUntilDashOrFullLine = LineUntilDash.Or(Line);

		public static readonly Parser<string> LineUntilUnderscore = Parse.AnyChar.Until(Underscore).Text();
		public static readonly Parser<string> LineUntilUnderscoreOrFullLine = LineUntilUnderscore.Or(Line);

		public static readonly Parser<string> LineUntilOpenSquareBracket = Parse.AnyChar.Until(OpenSquareBracket).Text();
		public static readonly Parser<string> LineUntilOpenParenthesis = Parse.AnyChar.Until(OpenParenthesis).Text();
		public static readonly Parser<string> LineUntilSquareBracketOrParenthesis = LineUntilOpenSquareBracket.Or(LineUntilOpenParenthesis).Text();

		public static readonly Parser<string> LineUntilDigit = Parse.AnyChar.Until(Parse.Number).Text();
		public static readonly Parser<string> LineUntilDigitOrFullLine = LineUntilDigit.Or(Line);
		#endregion
		#region lexers
		#region lexers by identifiers
		public static readonly Parser<IEnumerable<string>> IdentifiersSeparatedByUnderscore = IdentifierUntilUnderscoreOrFullWord.Many();
		public static readonly Parser<IEnumerable<string>> IdentifiersSeparatedByDash = IdentifierUntilDashOrFullWord.Many();
		#endregion
		#region lexers by line
		public static readonly Parser<IEnumerable<string>> LinesSeparatedByUnderscore = LineUntilUnderscoreOrFullLine.Many();
		public static readonly Parser<IEnumerable<string>> LinesSeparatedByDash = LineUntilDashOrFullLine.Many();
		#endregion
		#endregion
	}
}