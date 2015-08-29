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

using System;

namespace FansubFileNameParser.Metadata
{
    /// <summary>
    /// Represents the video mode used, such as "1080p," "720p," "480p," etc...
    /// </summary>
    [Serializable]
    public enum VideoMode
    {
        /// <summary>
        /// The default video mode
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Designates 480i video
        /// </summary>
        FourEightyInterlaced = 1 << 0,
        /// <summary>
        /// Designates 480p video
        /// </summary>
        FourEightyProgressive = 1 << 1,
        /// <summary>
        /// Designates 576i video
        /// </summary>
        FiveSeventySixInterlaced = 1 << 2,
        /// <summary>
        /// Designates 720p video
        /// </summary>
        SevenTwentyProgressive = 1 << 3,
        /// <summary>
        /// Designates 1080i video
        /// </summary>
        TenEightyInterlaced = 1 << 4,
        /// <summary>
        /// Designates 1080p video
        /// </summary>
        TenEightyProgressive = 1 << 5,
        /// <summary>
        /// Designates 576p video
        /// </summary>
        FiveSeventySixProgressive = 1 << 6,
    }
}
