﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NLog.Common;

namespace Backend.NLogSetup.DataDogHQ
{
    public class DatadogHttpClient : IDatadogClient, IDisposable
    {
        private const string _content = "application/json";
        private const int _maxSize = 2 * 1024 * 1024 - 51; // Need to reserve space for at most 49 "," and "[" + "]"
        private const int _maxMessageSize = 256 * 1024;

        /// <summary>
        ///     Shared UTF8 encoder.
        /// </summary>
        private static readonly UTF8Encoding UTF8 = new UTF8Encoding();

        private readonly HttpClient _client;

        private readonly string _url;

        public DatadogHttpClient(string url, string apiKey) 
        {
            _client = new HttpClient();
            _url = $"{url}/v1/input/{apiKey}";
            InternalLogger.Info("Creating HTTP client with config: {0}", _url);
        }

        public Task WriteAsync(IReadOnlyCollection<string> events) => Task.WhenAll(DoWrite(events));
        public void Write(IReadOnlyCollection<string> events) => Task.WhenAll(DoWrite(events)).GetAwaiter().GetResult();

        private IEnumerable<Task> DoWrite(IReadOnlyCollection<string> events)
        {
            var chunks = SerializeEvents(events);
            var tasks = chunks.Select(Post);
            return tasks;
        }

        public void Close() => _client?.Dispose();

        private static IList<string> SerializeEvents(IReadOnlyCollection<string> events)
        {
            var chunks = new List<string>();
            var currentSize = 0;

            var chunkBuffer = new List<string>(events.Count);
            foreach (var formattedLog in events)
            {
                var logSize = Encoding.UTF8.GetByteCount(formattedLog);
                if (logSize > _maxMessageSize) continue; // The log is dropped because the backend would not accept it
                if (currentSize + logSize > _maxSize)
                {
                    // Flush the chunkBuffer to the chunks and reset the chunkBuffer
                    chunks.Add(GenerateChunk(chunkBuffer, ",", "[", "]", currentSize));
                    chunkBuffer.Clear();
                    currentSize = 0;
                }

                chunkBuffer.Add(formattedLog);
                currentSize += logSize;
            }

            chunks.Add(GenerateChunk(chunkBuffer, ",", "[", "]", currentSize));
            return chunks;
        }

        private static string GenerateChunk(IList<string> collection, string delimiter, string prefix, string suffix, int size)
        {
            // The code below is optimized for performance:
            if (collection.Count <= 0)
                return prefix + suffix;

            var capacity = size + prefix.Length + suffix.Length + (collection.Count * delimiter.Length);
            var buf = new StringBuilder(capacity);
            buf.Append(prefix);
            buf.Append(collection[0]);
            for (var i = 1; i < collection.Count; ++i)
            {
                buf.Append(delimiter);
                buf.Append(collection[i]);
            }
            buf.Append(suffix);
            return buf.ToString();
            //return prefix + string.Join(delimiter, collection) + suffix;
        }

        private async Task Post(string payload)
        {
            var content = new StringContent(payload, Encoding.UTF8, _content);
            int? status = null;
            try
            {
                InternalLogger.Trace("Sending payload to Datadog: {0}", payload);
                var result = await _client.PostAsync(_url, content).ConfigureAwait(false);
                status = result == null ? null : (int?) result.StatusCode;
                InternalLogger.Trace("Statuscode: {0}", status);
                if (result?.IsSuccessStatusCode == true) 
                    return;
            }
            catch (Exception e)
            {
                InternalLogger.Error(status == null ? "Error sending payload (protocol=HTTP):\n{1}" :
                    "Error sending payload (protocol=HTTP, status={0}):\n{1}", status, e);
            }
            throw new CannotSendLogEventException();
        }

        public void Dispose() => Close();
    }
}