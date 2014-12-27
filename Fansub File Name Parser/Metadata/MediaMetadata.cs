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

using System;
using System.Runtime.Serialization;
using System.Text;

namespace FansubFileNameParser.Metadata
{
    /// <summary>
    /// Represents the media metadata that is encoded into a fansub file name
    /// </summary>
    [Serializable]
    public sealed class MediaMetadata : IEquatable<MediaMetadata>, ISerializable
    {
        #region private fields
        private readonly string AudioCodecKey = "Audio Codec";
        private readonly string CRC32Key = "CRC 32";
        private readonly string PixelBitDepthKey = "Pixel Bit Depth";
        private readonly string ResolutionKey = "Resolution";
        private readonly string VideoCodecKey = "Video Codec";
        private readonly string VideoMediaKey = "Video Media";
        private readonly string VideoModeKey = "Video Meode";
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
            Resolution = new Resolution();
            VideoCodec = string.Empty;
            VideoMedia = string.Empty;
            VideoMode = string.Empty;
        }

        private MediaMetadata(SerializationInfo info, StreamingContext context)
        {
            AudioCodec = info.GetString(AudioCodecKey);
            CRC32 = info.GetString(CRC32Key);
            PixelBitDepth = info.GetString(PixelBitDepthKey);
            Resolution = (Resolution)info.GetValue(ResolutionKey, typeof(Resolution));
            VideoCodec = info.GetString(VideoCodecKey);
            VideoMedia = info.GetString(VideoMediaKey);
            VideoMode = info.GetString(VideoModeKey);
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
        public Resolution Resolution { get; set; }

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
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
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

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object other)
        {
            if (EqualsPreamble(other) == false)
            {
                return false;
            }

            return Equals(other as MediaMetadata);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
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

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(MediaMetadata other)
        {
            if (EqualsPreamble(other) == false)
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

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(AudioCodecKey, AudioCodec);
            info.AddValue(CRC32Key, CRC32);
            info.AddValue(PixelBitDepthKey, PixelBitDepth);
            info.AddValue(ResolutionKey, Resolution);
            info.AddValue(VideoCodecKey, VideoCodec);
            info.AddValue(VideoMediaKey, VideoMedia);
            info.AddValue(VideoModeKey, VideoMode);
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