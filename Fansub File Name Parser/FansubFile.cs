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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FansubFileNameParser
{
	/// <summary>
	/// Represents a media file that was produced by a Fansub group
	/// </summary>
	[Serializable]
	public sealed class FansubFile : IEquatable<FansubFile>, ISerializable
	{
		#region private static readonly serialization keys
		private static readonly string FansubGroupKey = "FansubGroupKey";
		private static readonly string SeriesNameKey = "SeriesName";
		private static readonly string EpisodeNumberKey = "EpisodeNumberKey";
		private static readonly string ExtensionKey = "ExtensionKey";
		#endregion

		/// <summary>
		/// The name of the fansub group
		/// </summary>
		public string FansubGroup { get; private set; }
		/// <summary>
		/// The name of the anime series
		/// </summary>
		public string SeriesName { get; private set; }
		/// <summary>
		/// The episode number
		/// </summary>
		public int EpisodeNumber { get; private set; }
		/// <summary>
		/// The file extension of the media file
		/// </summary>
		public string Extension { get; private set; }

		/// <summary>
		/// Constructs a new immutable FansubFile object. You almost certainly won't be constructing these yourself. 
		/// <seealso cref="FansubFileParsers"/>
		/// </summary>
		/// <param name="fansubGroup">The fansub group name</param>
		/// <param name="seriesName">The anime series name</param>
		/// <param name="episodeNumber">
		/// The episode number. If this isn't applicable, then <see cref="int.MinValue"/> should be used
		/// </param>
		/// <param name="extension">The file extension</param>
		public FansubFile(string fansubGroup, string seriesName, int episodeNumber, string extension)
		{
			FansubGroup = fansubGroup;
			SeriesName = seriesName;
			EpisodeNumber = episodeNumber;
			Extension = extension;
		}

		/// <summary>
		/// Constructs a <see cref="FansubFile"/> based on the streaming
		/// </summary>
		/// <param name="streamingInfo">The serialization info object</param>
		/// <param name="context">The streaming context</param>
		public FansubFile(SerializationInfo streamingInfo, StreamingContext context)
			: this(
			streamingInfo.GetString(FansubGroupKey), 
			streamingInfo.GetString(SeriesNameKey), 
			streamingInfo.GetInt32(EpisodeNumberKey), 
			streamingInfo.GetString(ExtensionKey))
		{
		}

		/// <summary>
		/// Makes a deep opy this <see cref="FansubFile"/>.
		/// </summary>
		/// <returns>A fresh <see cref="FansubFile"/>.</returns>
		public FansubFile DeepCopy()
		{
			return new FansubFile(FansubGroup, SeriesName, EpisodeNumber, Extension);
		}

		/// <summary>
		/// Determines if an object is equal to this <see cref="FansubFile"/>.
		/// </summary>
		/// <param name="right">The other object.</param>
		/// <returns>True if they are equal. False otherwise.</returns>
		public override bool Equals(object right)
		{
			if (object.ReferenceEquals(right, null)) return false;

			if (object.ReferenceEquals(this, right)) return true;

			if (this.GetType() != right.GetType()) return false;

			return this.Equals(right as FansubFile);
		}

		/// <summary>
		/// Determines whether two FansubFiles are equal
		/// </summary>
		/// <param name="other">The other FansubFile</param>
		/// <returns>True if the files are equal. False otherwise</returns>
		public bool Equals(FansubFile other)
		{
			return FansubGroup.Equals(other.FansubGroup)
				&& SeriesName.Equals(other.SeriesName)
				&& EpisodeNumber == other.EpisodeNumber
				&& Extension.Equals(other.Extension);
		}

		/// <summary>
		/// Get the hash code of this file
		/// </summary>
		/// <returns>The hash code</returns>
		public override int GetHashCode()
		{
			return FansubGroup.GetHashCode() ^ SeriesName.GetHashCode() ^ EpisodeNumber ^ Extension.GetHashCode();
		}

		/// <summary>
		/// Gets the data used for serializing this object
		/// </summary>
		/// <param name="info">The serialization info object</param>
		/// <param name="context">The streaming context for this object</param>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(FansubGroupKey, FansubGroup);
			info.AddValue(SeriesNameKey, SeriesName);
			info.AddValue(EpisodeNumberKey, EpisodeNumber);
			info.AddValue(ExtensionKey, Extension);
		}
	}
}
