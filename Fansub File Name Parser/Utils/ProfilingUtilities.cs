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

using Newtonsoft.Json;
using Sprache;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FansubFileNameParser.Utils
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class ProfileRecord
    {
        [JsonProperty(PropertyName = "ParserName", Required = Required.Always)]
        public string ParserName { get; set; }
        [JsonProperty(PropertyName = "RunningTickCount", Required = Required.Always)]
        public long RunningTickCount { get; set; }
        [JsonProperty(PropertyName = "RunningCallCount", Required = Required.Always)]
        public int RunningCallCount { get; set; }
    }

    internal sealed class Profiler
    {
        private readonly IDictionary<string, ProfileRecord> _profileRecords = new Dictionary<string, ProfileRecord>();

        public void AddOrUpdateProfile(string parserName, long tickCount)
        {
            ProfileRecord record;
            if (_profileRecords.TryGetValue(parserName, out record) == false)
            {
                record = new ProfileRecord
                {
                    ParserName = parserName,
                    RunningTickCount = 0,
                    RunningCallCount = 0,
                };

                _profileRecords.Add(parserName, record);
            }

            record.RunningCallCount = record.RunningCallCount + 1;
            record.RunningTickCount = record.RunningTickCount + tickCount;
        }

        public Dictionary<string, ProfileRecord> GetProfileRecord()
        {
            return new Dictionary<string, ProfileRecord>(_profileRecords);
        }
    }

    /// <summary>
    /// A class of Profiling tools used to measure the performance of Parsers
    /// </summary>
    public static class ProfilingUtilities
    {
        private static readonly Profiler Profiler = new Profiler();

        /// <summary>
        /// Adds profiling data to the parser
        /// </summary>
        /// <typeparam name="TResult">The result of the Parser</typeparam>
        /// <param name="this">The Parser</param>
        /// <param name="parserName">The name of the parser</param>
        /// <returns>A new parser that adds profiling data</returns>
        public static Parser<TResult> Profile<TResult>(this Parser<TResult> @this, string parserName)
        {
            return input =>
            {
#if PROFILE
                var timer = Stopwatch.StartNew();
#endif
                var result = @this.Invoke(input);
#if PROFILE
                var ticks = timer.ElapsedTicks;

                Profiler.AddOrUpdateProfile(parserName, ticks);
#endif
                return result;
            };
        }

        /// <summary>
        /// Dumps the profile data by returning it as a string
        /// </summary>
        /// <returns>The profile data represented as a string</returns>
        public static string DumpProfileData()
        {
            return JsonConvert.SerializeObject(Profiler.GetProfileRecord());
        }
    }
}
