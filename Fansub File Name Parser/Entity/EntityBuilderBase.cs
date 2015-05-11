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
using System;
using System.Collections.Generic;

namespace FansubFileNameParser.Entity
{
    /// <summary>
    /// The base class for all <see cref="IEntityBuilder{T}"/>s
    /// </summary>
    /// <typeparam name="T">The type of <see cref="IFansubEntity"/> this builder produces</typeparam>
    internal abstract class EntityBuilderBase<T> : IEntityBuilder<T> where T : IFansubEntity
    {
        #region private classes
        private sealed class BasePropertySetterVisitor : IFansubEntityVisitor
        {
            public Maybe<string> Series { get; set; }
            public Maybe<string> Group { get; set; }

            private void SetFields(FansubEntityBase entity)
            {
                entity.Series = Series;
                entity.Group = Group;
            }

            public void Visit(FansubEpisodeEntity entity)
            {
                SetFields(entity);
            }

            public void Visit(FansubOPEDEntity entity)
            {
                SetFields(entity);
            }

            public void Visit(FansubOriginalAnimationEntity entity)
            {
                SetFields(entity);
            }

            public void Visit(FansubMovieEntity entity)
            {
                SetFields(entity);
            }

            public void Visit(Directory.FansubDirectoryEntity entity)
            {
                SetFields(entity);
            }
        }
        #endregion

        #region protected classes
        /// <summary>
        /// A parsed token from the string
        /// </summary>
        protected sealed class ParseToken
        {
            /// <summary>
            /// Gets or sets the index at which the token was retrieved
            /// </summary>
            /// <value>
            /// The index at which the token was retrieved
            /// </value>
            public int Index { get; set; }

            /// <summary>
            /// Gets or sets the token.
            /// </summary>
            /// <value>
            /// The token.
            /// </value>
            public string Token { get; set; }
        }
        #endregion

        #region private fields
        private string _inputString;
        private Maybe<string> _seriesName;
        private Maybe<string> _groupName;
        #endregion

        #region public methods
        /// <summary>
        /// Sets the input string that needs to be parsed in order to build this entity
        /// </summary>
        /// <param name="inputString">The input string.</param>
        public void SetInputString(string inputString)
        {
            _inputString = inputString;
        }

        /// <summary>
        /// Builds the <see cref="IFansubEntity"/> with the given string
        /// </summary>
        /// <returns>The constructed <see cref="IFansubEntity"/></returns>
        public T Build()
        {
            if (_inputString == null)
            {
                throw new InvalidOperationException("Input string must be set. Call SetInputString() before calling Build()");
            }

            var unprocessedTokens = new List<ParseToken>();
            foreach (var token in GenerateTokens())
            {
                var didProcessToken = TryProcessToken(token);
                if (didProcessToken == false)
                {
                    unprocessedTokens.Add(token);
                }
            }

            // Try to postprocess the unprocessed tokens for the fansub name and the series name.
            // Child parsers should not parse any tokens that we can parse here
            PostProcessTokens(unprocessedTokens);

            var constructedEntity = ConstructEntity();
            
            SetCommonFields(constructedEntity);

            return constructedEntity;
        }
        #endregion

        #region protected methods
        /// <summary>
        /// <para>
        /// Attempt to process the token. If the token cannot be used to provide information for this particular
        /// builder, then this function must return FALSE. 
        /// 
        /// The same token can be sent twice.
        /// </para>
        /// </summary>
        /// <param name="token">The string token.</param>
        /// <returns>
        /// <para>
        /// True if a token was successfully processed and will be used as a piece of
        /// information; it MUST result in setting a field for an <see cref="IFansubEntity"/>. 
        /// False otherwise
        /// </para>
        /// </returns>
        protected abstract bool TryProcessToken(ParseToken token);

        /// <summary>
        /// Constructs the <see cref="IFansubEntity"/>
        /// </summary>
        /// <returns>The <see cref="IFansubEntity"/></returns>
        protected abstract T ConstructEntity();
        #endregion

        #region private methods
        private IEnumerable<ParseToken> GenerateTokens()
        {
            yield break;
        }

        private void SetCommonFields(IFansubEntity entity)
        {
            var fieldSetter = new BasePropertySetterVisitor
            {
                Series = _seriesName,
                Group = _groupName,
            };

            entity.Accept(fieldSetter);
        }

        /// <summary>
        /// Tries to preprocess the string token to see if it can extract any common elements across all <see cref="IFansubEntities"/>, like 
        /// the fansub group or the series name
        /// </summary>
        /// <param name="token">The string token.</param>
        /// <returns>True if we preprocessed a field successfully. False otherwise</returns>
        private void PostProcessTokens(List<ParseToken> tokens)
        {
            foreach(var token in tokens)
            {
                if (_groupName.IsSomething() && _seriesName.IsSomething())
                {
                    return;
                }

                if (_groupName.IsNothing())
                {
                    _groupName = TryGetFansubGroup(token);
                }

                if (_seriesName.IsNothing())
                {
                    _seriesName = TryGetSeriesName(token);
                }
            }
        }

        /// <summary>
        /// Tries the get fansub group.
        /// </summary>
        /// <param name="token">The string token.</param>
        /// <returns>The fansub group</returns>
        private static Maybe<string> TryGetFansubGroup(ParseToken token)
        {
            return Maybe<string>.Nothing;
        }

        /// <summary>
        /// Tries the name of the get series.
        /// </summary>
        /// <param name="token">The string token.</param>
        /// <returns>The series name</returns>
        private static Maybe<string> TryGetSeriesName(ParseToken token)
        {
            return Maybe<string>.Nothing;
        }
        #endregion
    }
}
