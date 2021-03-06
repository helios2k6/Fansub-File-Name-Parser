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

using FansubFileNameParser.Utils;
using Functional.Maybe;
using Sprache;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FansubFileNameParser
{
    /// <summary>
    /// Extra parsers that perform specialized parsing and safe type deduction
    /// </summary>
    internal static class ExtraParsers
    {
        #region Metatag content
        /// <summary>
        /// Parses the content of a metatag
        /// </summary>
        public static Parser<string> MetaTagContent
        {
            get { return LineUpToTagDeliminator.Contained(BaseGrammars.OpenTagDeliminator, BaseGrammars.ClosedTagDeliminator); }
        }

        /// <summary>
        /// Parses multiple metadata tags
        /// </summary>
        public static Parser<IEnumerable<string>> MetaTagGroup
        {
            get
            {
                return from firstTag in MetaTagContent.AsMany()
                       from remainingTags in MetaTagContent.Token().Many()
                       select firstTag.Concat(remainingTags);
            }
        }
        #endregion
        #region Integer
        /// <summary>
        /// Parses a number from a string and safely casts it to an integer
        /// </summary>
        public static Parser<int> Int
        {
            get
            {
                return from intAsString in Parse.Number
                       let parseResult = TryParseInt(intAsString)
                       where parseResult.IsSomething()
                       select parseResult.Value;
            }
        }

        private static Maybe<int> TryParseInt(string input)
        {
            int output;
            if (int.TryParse(input, out output))
            {
                return output.ToMaybe();
            }

            return Maybe<int>.Nothing;
        }
        #endregion
        #region Episode and version numbers
        /// <summary>
        /// Parses a version number token
        /// </summary>
        public static Parser<int> VersionNumber
        {
            get
            {
                return from _1 in Parse.Char('v').Or(Parse.Char('V'))
                       from number in Int
                       select number;
            }
        }

        /// <summary>
        /// Parses an episode with an optional version number on it
        /// </summary>
        public static Parser<Tuple<int, Maybe<int>>> EpisodeWithVersionNumber
        {
            get
            {
                return from episodeNumber in Int
                       from versionNumber in VersionNumber.OptionalMaybe()
                       select Tuple.Create(episodeNumber, versionNumber);
            }
        }

        /// <summary>
        /// Parses a episode token of the form {whitespace}{Episode and Version}
        /// </summary>
        public static Parser<Tuple<int, Maybe<int>>> EpisodeVersionWithSpaceToken
        {
            get
            {
                return from _1 in Parse.WhiteSpace
                       from episodeAndVersion in EpisodeWithVersionNumber
                       select episodeAndVersion;
            }
        }
        #endregion
        #region Lines and Strings
        /// <summary>
        /// Parses the beginning of a line. On success, this returns the empty string. By 
        /// definition, it does not consume any input
        /// </summary>
        public static Parser<string> BeginnningOfLine
        {
            get
            {
                return input =>
                {
                    if (input.Position == 0)
                    {
                        return Result.Success<string>(string.Empty, input);
                    }
                    return Result.Failure<string>(input, "Could not parse the beginning of the line", Enumerable.Empty<string>());
                };
            }
        }

        /// <summary>
        /// Removes beginning and trailing whitespace from the input string. 
        /// 
        /// Does NOT consume any input and the remainder string is equal to the input string. 
        /// You usually want to combine this parser with the <seealso cref="SetResultAsRemainder"/>
        /// function.
        /// 
        /// This function always succeeds, unless the input is already out of bounds, in which case, this
        /// parser will throw an exception
        /// </summary>
        public static Parser<string> Trim
        {
            get
            {
                return input =>
                {
                    return Result.Success<string>(input.Source.Substring(input.Position).Trim(), input);
                };
            }
        }
        #endregion
        #region Parsers and Extensions
        #region Utilities
        /// <summary>
        /// Converts a Parser into a bool, mapping a successful parse to "true" and
        /// any other result to "false." 
        /// </summary>
        /// <remarks>
        /// This function ALWAYS succeeds, just like Optional{T}, but it simply maps the
        /// optional result to a true or false. This parser will consume input exactly like
        /// any Optional{T} parser would; it consumes the necessary input upon success and
        /// no input on failure.
        /// </remarks>
        /// <typeparam name="TResult">The type of the result from the Parser</typeparam>
        /// <param name="this">The Parser</param>
        /// <returns>A new parser that returns true or false depending on the parse result</returns>
        public static Parser<bool> WasSuccessful<TResult>(this Parser<TResult> @this)
        {
            return input =>
            {
                var result = @this.Invoke(input);
                if (result.WasSuccessful)
                {
                    return Result.Success<bool>(true, result.Remainder);
                }

                return Result.Success<bool>(false, input);
            };
        }

        /// <summary>
        /// Invokes this parser and upon success will set the remainder string to whatever
        /// this parser returned.
        /// 
        /// On failure, this will return the result of the failed parse
        /// </summary>
        /// <param name="this">The parser.</param>
        /// <returns>A new parser that sets the remainder as the value</returns>
        public static Parser<string> SetResultAsRemainder(this Parser<string> @this)
        {
            return input =>
            {
                var result = @this.Invoke(input);
                if (result.WasSuccessful == false)
                {
                    return result;
                }

                return Result.Success<string>(result.Value, new Input(result.Value));
            };
        }

        /// <summary>
        /// Memoizes the specified <see cref="Parser{T}"/>
        /// </summary>
        /// <typeparam name="TResult">The type of result</typeparam>
        /// <param name="this">The parser</param>
        /// <returns>A memoized version of this parser</returns>
        public static Parser<TResult> Memoize<TResult>(this Parser<TResult> @this)
        {
            var memopad = new ConcurrentDictionary<IInput, IResult<TResult>>();
            return input => memopad.GetOrAdd(input, @this.Invoke);
        }

        /// <summary>
        /// Constructs a parser that will only succeed if it consumes the last possible token that satisfies this 
        /// parser
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="this">The parser</param>
        /// <returns>A new parser that will succeed only if it parses the last token that satisfies this parser</returns>
        public static Parser<TResult> Last<TResult>(this Parser<TResult> @this)
        {
            return from f in @this
                   from _ in ScanFor(@this).Not()
                   select f;
        }

        /// <summary>
        /// Constructs a parser that indicates the given parser is optional. The only difference between the 
        /// traditional Parser{TResult}.Optional() construct is that this parser will return a Maybe{TResult} instead of
        /// an IOption{TResult}
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="this">The parser.</param>
        /// <returns>A new parser that returns a Maybe{TResult}</returns>
        public static Parser<Maybe<TResult>> OptionalMaybe<TResult>(this Parser<TResult> @this)
        {
            return from r in @this.Optional()
                   select r.ConvertFromIOptionToMaybe();
        }

        /// <summary>
        /// Implodes the <see cref="Parser{T}"/>'s <see cref="IEnumerable{T}"/> with the specified aggregator
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="this">The parser.</param>
        /// <param name="seed">The seed value.</param>
        /// <param name="accumulator">The accumulator function.</param>
        /// <returns>A <see cref="Parser{T}"/> that will aggregate the results afterwards</returns>
        public static Parser<TResult> Implode<TInput, TResult>(
            this Parser<IEnumerable<TInput>> @this,
            TResult seed,
            Func<TResult, TInput, TResult> accumulator
        )
        {
            return input =>
            {
                var result = @this.Invoke(input);
                if (result.WasSuccessful == false)
                {
                    return Result.Failure<TResult>(
                        result.Remainder,
                        string.Format("Failure to implode. Dependent parser failed. {0}", result.Message),
                        result.Expectations
                    );
                }
                return Result.Success<TResult>(result.Value.Aggregate<TInput, TResult>(seed, accumulator), result.Remainder);
            };
        }

        /// <summary>
        /// Transforms the result of a Parser{T} to be Parser{IEnumerable{T}}. This does not
        /// parse any additional tokens beyond what the underlying Parser{T} parses. This merely
        /// allows you to use a Parser{T} as a Parser{IEnumerable{T}}
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="this">The parser</param>
        /// <returns>
        /// A new parser that returns an IEnumerable{T} with the token parsed by the originalParser{T}
        /// </returns>
        /// <remarks>
        /// This does not work like the Many() function! Unlike the Many() parser, this parser will fail if the 
        /// underlying parser fails
        /// </remarks>
        public static Parser<IEnumerable<T>> AsMany<T>(this Parser<T> @this)
        {
            return input =>
            {
                var result = @this.Invoke(input);
                if (result.WasSuccessful)
                {
                    return Result.Success<IEnumerable<T>>(new[] { result.Value }, result.Remainder);
                }

                return Result.Failure<IEnumerable<T>>(
                    input,
                    string.Format("The underlying Parser did not succeed: {0}", result.Message),
                    result.Expectations
                );
            };
        }
        #endregion
        #region Strings
        /// <summary>
        /// Converts a Parser{char} to a Parser{string}
        /// </summary>
        /// <param name="this">The Parser</param>
        /// <returns>A new Parser that returns a string</returns>
        public static Parser<string> AsString(this Parser<char> @this)
        {
            return input =>
            {
                var result = @this.Invoke(input);
                if (result.WasSuccessful)
                {
                    return Result.Success<string>(new string(result.Value, 1), result.Remainder);
                }

                return Result.Failure<string>(result.Remainder, result.Message, result.Expectations);
            };
        }

        /// <summary>
        /// Creates a parser that will only succeed if the return result has at least 1 alphanumeric 
        /// character.
        /// </summary>
        /// <param name="this">The principle parser</param>
        /// <returns>A new parser</returns>
        public static Parser<string> AtLeastOneCharTrimmed(this Parser<string> @this)
        {
            return input =>
            {
                var result = @this.Invoke(input);
                if (result.WasSuccessful)
                {
                    if (result.Value.Trim().Length <= 0)
                    {
                        return Result.Failure<string>(
                            input,
                            "The parsed result had less than 1 alphanumeric character",
                            Enumerable.Empty<string>()
                         );
                    }
                }

                return result;
            };
        }

        /// <summary>
        /// Evalutes this parser and then consumes the rest of the input and discards it
        /// </summary>
        /// <typeparam name="TResult">The result of this parser</typeparam>
        /// <param name="this">The parser</param>
        /// <returns>A new parser that evalutes this parser and then consumes the remaining input</returns>
        public static Parser<TResult> ConsumeAllRemainingInput<TResult>(this Parser<TResult> @this)
        {
            return from result in @this
                   from _ in BaseGrammars.Line
                   select result;
        }
        #endregion
        #region Continuations
        /// <summary>
        /// Parses the antecedent parser and feeds its results into the <paramref name="continuation" /> parser
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="this">The antecedent parser</param>
        /// <param name="converter">The converter function to link the two parsers together.</param>
        /// <param name="continuation">The continuation parser.</param>
        /// <returns>
        /// A new parser that is the parsing of the antecedent's result using the continuation parser
        /// </returns>
        public static Parser<TResult> ContinueWith<TInput, TResult>(
            this Parser<TInput> @this,
            Func<TInput, string> converter,
            Parser<TResult> continuation
        )
        {
            return input =>
            {
                var result = @this.Invoke(input);
                if (result.WasSuccessful == false)
                {
                    return Result.Failure<TResult>(
                        result.Remainder,
                        string.Format("Failure to continue with the continuation parser. The antecedent parser failed. {0}", result.Message),
                        result.Expectations
                    );
                }

                return continuation.Invoke(new Input(converter.Invoke(result.Value)));
            };
        }

        /// <summary>
        /// Parses the antecedent parser and feeds its results into the <paramref name="continuation" /> parser
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="this">The antecedent parser.</param>
        /// <param name="continuation">The continuation parser.</param>
        /// <returns>
        /// A new parser that is the parsing of the antecedent's result using the continuation parser
        /// </returns>
        public static Parser<TResult> ContinueWith<TResult>(
            this Parser<string> @this,
            Parser<TResult> continuation
        )
        {
            return @this.ContinueWith(i => i, continuation);
        }

        /// <summary>
        /// Returns a new parser that will invoke this parser and then reset the input back to what it was when it
        /// was fed into this parser
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="this">The parser.</param>
        /// <returns>A new parser that resets its input on completion</returns>
        public static Parser<TResult> ResetInput<TResult>(this Parser<TResult> @this)
        {
            return input =>
            {
                var result = @this.Invoke(input);
                return result.WasSuccessful
                    ? Result.Success<TResult>(result.Value, input)
                    : Result.Failure<TResult>(input, result.Message, result.Expectations);
            };
        }
        #endregion
        #endregion
        #region Standalone Parsers
        #region Utilities
        /// <summary>
        /// Parses a file extension
        /// </summary>
        public static Parser<string> FileExtension
        {
            get
            {
                return from dot in BaseGrammars.Dot
                       from extContent in Parse.AnyChar.Except(BaseGrammars.Dot.Or(BaseGrammars.TagDeliminator)).Many().End().Text()
                       select string.Concat(dot, extContent);
            }
        }
        #region filters
        /// <summary>
        /// Replaces the dots in the string with spaces while preserving the file extension of a media file
        /// </summary>
        public static Parser<string> ReplaceDotsExceptMediaFileExtension
        {
            get
            {
                return from frontSegment in Parse.AnyChar.Except(FileExtension).Many().Text()
                       from extension in FileExtension.Optional()
                       select string.Format(
                            "{0}{1}",
                            frontSegment.Replace('.', ' '),
                            extension.IsDefined
                                ? extension.Get()
                                : string.Empty
                       );
            }
        }

        /// <summary>
        /// Constructs a filtering parser that removes anything the given parser successfully parses.
        /// This parser consumes all characters
        /// </summary>
        /// <param name="filter">The filter parser</param>
        /// <returns>A newly constructed parser that filters tokens</returns>
        public static Parser<string> Filter<TResult>(Parser<TResult> filter)
        {
            var innerLoop = from mainToken in Parse.AnyChar.Except(filter).Many().Text()
                            from _ in filter.Optional()
                            select mainToken.Trim();

            return innerLoop.Many().Implode(
                string.Empty,
                (acc, i) => string.IsNullOrWhiteSpace(acc)
                    ? i
                    : string.Format("{0} {1}", acc, i)
            );
        }

        /// <summary>
        /// Replaces the underscores of an input string with spaces
        /// </summary>
        public static Parser<string> ReplaceUnderscores
        {
            get { return Filter(BaseGrammars.Underscore); }
        }

        /// <summary>
        /// Sanitizes the input string by replacing dots and dashes with spaces, with the exception 
        /// of any file extensions
        /// </summary>
        public static Parser<string> CleanInputString
        {
            get { return ReplaceDotsExceptMediaFileExtension.ContinueWith(ReplaceUnderscores).Profile("CleanInputStringParser"); }
        }
        #endregion
        /// <summary>
        /// Creates a No-Op failure parser
        /// </summary>
        /// <typeparam name="TResult">The return type of this parser</typeparam>
        /// <returns>A new parser that parses nothing and always fails</returns>
        public static Parser<TResult> CreateNoOpFailureParser<TResult>()
        {
            return i => Result.Failure<TResult>(i, "No-Op Failure Parser", Enumerable.Empty<string>());
        }

        /// <summary>
        /// Parses any character that isn't a letter
        /// </summary>
        public static Parser<char> NonLetterAndNonNumber = Parse.Char(c => !char.IsLetterOrDigit(c), "Non-Letter And Non-Number");

        /// <summary>
        /// Cycles through each parser in the IEnumerable{Parser{TResult}} and will return
        /// the result of the first one that successfully parses the input string
        /// </summary>
        /// <typeparam name="TResult">The type of result to parse</typeparam>
        /// <param name="parsers">The IEnumerable of parsers</param>
        /// <returns>A single parser that will enumerate all of the given parsers</returns>
        public static Parser<TResult> Any<TResult>(IEnumerable<Parser<TResult>> parsers)
        {
            return input =>
            {
                foreach (var parser in parsers)
                {
                    var result = parser.Invoke(input);
                    if (result.WasSuccessful)
                    {
                        return result;
                    }
                }

                return Result.Failure<TResult>(
                    input,
                    "None of the provided parsers successfully parsed the input",
                    Enumerable.Empty<string>()
                );
            };
        }
        #endregion
        #region Boolean Operators
        /// <summary>
        /// Coalesces two parsers, which may have different return types, into a single parser that succeeds if
        /// either of the parsers succeeds. 
        /// </summary>
        /// <typeparam name="TFirst">The return type of the first parser</typeparam>
        /// <typeparam name="TSecond">The return type of the second parser</typeparam>
        /// <param name="first">The first parser</param>
        /// <param name="second">The second parser</param>
        /// <returns>
        /// A new parser that will succeed if either parser succeeds. The return value of the parser is a bool, 
        /// where "true" means one of the parsers succeeded. This parser, never returns false
        /// </returns>
        public static Parser<bool> Or<TFirst, TSecond>(
            Parser<TFirst> first,
            Parser<TSecond> second
        )
        {
            return Or(first, second, CreateNoOpFailureParser<bool>());
        }

        /// <summary>
        /// Coalesces three parsers, which may have different return types, into a single parser that succeeds if 
        /// any of the parsers succeeds.
        /// </summary>
        /// <typeparam name="TFirst">The return type of the first parser</typeparam>
        /// <typeparam name="TSecond">The return type of the second parser</typeparam>
        /// <typeparam name="TThird">The return type of the third parser</typeparam>
        /// <param name="first">The first parser</param>
        /// <param name="second">The second parser</param>
        /// <param name="third">The third parser</param>
        /// <returns>
        /// A new parser that will succeed if any parser succeeds. The return value of the parser is a bool,
        /// where true means one of the parsers succeded and "false" means no parser succeeded
        /// </returns>
        public static Parser<bool> Or<TFirst, TSecond, TThird>(
            Parser<TFirst> first,
            Parser<TSecond> second,
            Parser<TThird> third
        )
        {
            return Or(first, second, third, CreateNoOpFailureParser<bool>());
        }

        /// <summary>
        /// Coalesces three parsers, which may have different return types, into a single parser that succeeds if 
        /// any of the parsers succeeds.
        /// </summary>
        /// <typeparam name="TFirst">The return type of the first parser</typeparam>
        /// <typeparam name="TSecond">The return type of the second parser</typeparam>
        /// <typeparam name="TThird">The return type of the third parser</typeparam>
        /// <typeparam name="TFourth">The return type of the fourth parser</typeparam>
        /// <param name="first">The first parser</param>
        /// <param name="second">The second parser</param>
        /// <param name="third">The third parser</param>
        /// <param name="fourth">The fourth parser</param>
        /// <returns>
        /// A new parser that will succeed if any parser succeeds. The return value of the parser is a bool,
        /// where true means one of the parsers succeded and "false" means no parser succeeded
        /// </returns>
        public static Parser<bool> Or<TFirst, TSecond, TThird, TFourth>(
            Parser<TFirst> first,
            Parser<TSecond> second,
            Parser<TThird> third,
            Parser<TFourth> fourth
        )
        {
            return input =>
            {
                var result = first.Invoke(input);
                if (result.WasSuccessful)
                {
                    return Result.Success<bool>(true, result.Remainder);
                }

                var secondResult = second.Invoke(input);
                if (secondResult.WasSuccessful)
                {
                    return Result.Success<bool>(true, result.Remainder);
                }

                var thirdResult = third.Invoke(input);
                if (thirdResult.WasSuccessful)
                {
                    return Result.Success<bool>(true, result.Remainder);
                }

                var fourthResult = fourth.Invoke(input);
                if (fourthResult.WasSuccessful)
                {
                    return Result.Success<bool>(true, result.Remainder);
                }

                return Result.Failure<bool>(input, "None of the parsers succeeded", Enumerable.Empty<string>());
            };
        }
        #endregion
        #region Scanners
        /// <summary>
        /// Parses and captures all of the metadata tags that appear in the string, including the tag deliminator token
        /// </summary>
        public static Parser<IEnumerable<string>> CollectTags
        {
            get { return ScanFor(MetaTagContent).Many(); }
        }

        /// <summary>
        /// Returns a parser that will scan for a token that is parsable by the given parser
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="scanner">The parser.</param>
        /// <returns>A new parser that scans for any token that is parsable by the given parser</returns>
        public static Parser<TResult> ScanFor<TResult>(Parser<TResult> scanner)
        {
            return from _ in Parse.AnyChar.Except(scanner).Many().Text()
                   from s in scanner
                   select s;
        }

        /// <summary>
        /// Collects the string leading up to a token. It does NOT consume the token
        /// </summary>
        /// <typeparam name="TThrowAway">The type of result to throw away.</typeparam>
        /// <param name="excepter">The excepter parser.</param>
        /// <returns>A new parser that collects the string all the way up to a specific point</returns>
        public static Parser<string> LineUpTo<TThrowAway>(Parser<TThrowAway> excepter)
        {
            return Parse.AnyChar.Except(excepter).Many().Text();
        }

        /// <summary>
        /// Parses a line of text until a tag delmiinator is encountered
        /// </summary>
        public static Parser<string> LineUpToTagDeliminator
        {
            get { return LineUpTo(BaseGrammars.TagDeliminator); }
        }

        /// <summary>
        /// Parses content that's contained between any group of multiple tags
        /// </summary>
        private static Parser<string> ContentBetweenTagGroups
        {
            get { return LineUpToTagDeliminator.Contained(MetaTagGroup, MetaTagGroup); }
        }

        /// <summary>
        /// Parses the main content of a title, which excludes the metatags
        /// </summary>
        public static Parser<string> MainContent
        {
            get { return ContentBetweenTagGroups.Or(LineUpToTagDeliminator); }
        }
        #endregion
        #endregion
    }
}
