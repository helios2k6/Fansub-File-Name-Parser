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
    /// Represents the audio codec used to encode the audio stream of a media file
    /// </summary>
    [Serializable]
    public enum AudioCodec
    {
        /// <summary>
        /// Designates the default and unknown audio codec
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// The AAC audio codec
        /// </summary>
        AAC = 1 << 0,
        /// <summary>
        /// The AC3 audio codec
        /// </summary>
        AC3 = 1 << 1,
        /// <summary>
        /// The DTS audio codec
        /// </summary>
        DTS = 1 << 2,
        /// <summary>
        /// The FLAC audio codec
        /// </summary>
        FLAC = 1 << 3,
        /// <summary>
        /// The MP3 audio codec
        /// </summary>
        MP3 = 1 << 4,
        /// <summary>
        /// The OGG audio codec
        /// </summary>
        OGG = 1 << 5,
    }
}
