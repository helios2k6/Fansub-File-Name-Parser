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
using System.Runtime.Serialization;
using System.Text;
namespace FansubFileNameParser
{
    /// <summary>
    /// Represents the media metadata that is encoded into a fansub file name
    /// </summary>
    [Serializable]
    public sealed class MediaMetadata : IEquatable<MediaMetadata>, ISerializable
    {
        #region private fields

        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaMetadata"/> class.
        /// </summary>
        public MediaMetadata()
        {
            AudioCodec = string.Empty;
            CRC32 = string.Empty;
            PixelBitDepth = string.Empty;
            Resolution = string.Empty;
            VideoCodec = string.Empty;
            VideoMedia = string.Empty;
            VideoMode = string.Empty;
        }

        private MediaMetadata(SerializationInfo info, StreamingContext context)
        {

        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets or sets the audio codec.
        /// 
        /// Audio Codec equates to "AAC," FLAC," "DTS," etc...
        /// </summary>
        /// <value>
        /// The audio codec.
        /// </value>
        public string AudioCodec { get; set; }

        /// <summary>
        /// Gets or sets the CRC32 Checksum
        /// </summary>
        /// <value>
        /// The CRC32 Checksum.
        /// </value>
        public string CRC32 { get; set; }

        /// <summary>
        /// Gets or sets the Pixel bit depth.
        /// 
        /// Pixel Bit Depth equates to "8 bit" or "10 bit"
        /// </summary>
        /// <value>
        /// The pixel bit depth.
        /// </value>
        public string PixelBitDepth { get; set; }

        /// <summary>
        /// Gets or sets the resolution.
        /// 
        /// Resolution equates to "1280x720," "1920x1080," etc...
        /// </summary>
        /// <value>
        /// The resolution.
        /// </value>
        public string Resolution { get; set; }

        /// <summary>
        /// Gets or sets the video codec.
        /// 
        /// Video Codec equates to "H264," "XVID," etc...
        /// </summary>
        /// <value>
        /// The video codec.
        /// </value>
        public string VideoCodec { get; set; }

        /// <summary>
        /// Gets or sets the video media. 
        /// 
        /// Video Media equates to either "TV," "Bluray," "BD," "DVD," etc...
        /// </summary>
        /// <value>
        /// The video media.
        /// </value>
        public string VideoMedia { get; set; }

        /// <summary>
        /// Gets or sets the video mode. 
        /// 
        /// Video Mode equates to either "1080p," "720p," "480p," etc...
        /// </summary>
        /// <value>
        /// The video mode.
        /// </value>
        public string VideoMode { get; set; }
        #endregion

        #region public methods
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine("Media Metadata");
            builder.AppendLine("Audio Codec: " + AudioCodec);
            builder.AppendLine("CRC32 Checksum: " + CRC32);
            builder.AppendLine("Pixel Bit Depth: " + PixelBitDepth);
            builder.AppendLine("Resolution: " + Resolution);
            builder.AppendLine("Video Codec: " + VideoCodec);
            builder.AppendLine("Video Media: " + VideoMedia);
            builder.AppendLine("Video Mode: " + VideoMode);

            return builder.ToString();
        }

        public override bool Equals(object other)
        {
            if(EqualsPreamble(other) == false)
            {
                return false;
            }

            return Equals(other as MediaMetadata);
        }

        public override int GetHashCode()
        {
            return AudioCodec.GetHashCode()
                ^ CRC32.GetHashCode()
                ^ PixelBitDepth.GetHashCode()
                ^ Resolution.GetHashCode()
                ^ VideoCodec.GetHashCode()
                ^ VideoMedia.GetHashCode()
                ^ VideoMode.GetHashCode();
        }

        public bool Equals(MediaMetadata other)
        {
            if(EqualsPreamble(other) == false)
            {
                return false;
            }

            return Equals(AudioCodec, other.AudioCodec)
                && Equals(CRC32, other.CRC32)
                && Equals(PixelBitDepth, other.PixelBitDepth)
                && Equals(Resolution, other.Resolution)
                && Equals(VideoCodec, other.VideoCodec)
                && Equals(VideoMedia, other.VideoMedia)
                && Equals(VideoMode, other.VideoMode);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }
        #endregion

        #region private method
        private bool EqualsPreamble(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;

            return true;
        }
        #endregion
    }
}
