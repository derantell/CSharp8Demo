using Pastel;
using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Threading.Tasks;
using static System.Console;
using System.IO;

namespace CSharp8Demo {
    class Program {
        static async Task Main(string[] args) {
            using var urlSource = IsInputRedirected
                ? In
                : new StreamReader(args[0]);

            await foreach (var siteurl in GetSiteUrls(urlSource)) {
                WriteLine(
                    FormatResponse(
                        await GetTimedResponse(siteurl)));
            }
        }

        async static IAsyncEnumerable<string> GetSiteUrls(TextReader urlSource) {
            string siteurl;
            while (true) {
                siteurl = await urlSource.ReadLineAsync();
                if (siteurl != null) {
                    yield return siteurl;
                }
                else {
                    yield break;
                }
            }
        }

        static async Task<IResponseResult> GetTimedResponse(string url) {
            using var http = new HttpClient();
            IResponseResult result;
            try {
                Stopwatch watch = Stopwatch.StartNew();
                var response = await http.GetAsync(url);
                result = new ResponseTimeResult(
                    url,
                    (int)response.StatusCode,
                    watch.ElapsedMilliseconds
                );
            }
            catch (Exception e) {
                result = new ResponseError(url, e);
            }
            return result;
        }

        static string FormatResponse(IResponseResult result) =>
            result switch {
                ResponseTimeResult(var url, 200, var ms) when ms < 300 =>
                    $"- {url} - Excellent response time, {ms} ms"
                        .Pastel(Color.LightBlue),
                ResponseTimeResult(var url, 200, var ms) when ms < 800 =>
                    $"- {url} - Good response time, {ms} ms"
                        .Pastel(Color.ForestGreen),
                ResponseTimeResult(var url, 200, var ms) when ms < 2000 =>
                    $"- {url} - Ok response time, {ms} ms"
                        .Pastel(Color.Yellow),
                ResponseTimeResult(var url, 200, var ms) when ms >= 2000 =>
                    $"- {url} - Bad response time, {ms} ms"
                        .Pastel(Color.Orange),
                ResponseTimeResult(var url, 404, _) =>
                    $"- {url} - Not found"
                        .Pastel(Color.Red),
                ResponseTimeResult(var url, var status, _) when status >= 500 =>
                    $"- {url} - Server error ({status})"
                        .Pastel(Color.Red),
                ResponseError(var url, var ex) =>
                    $"x {url} - An exception occurred: {ex.Message}"
                        .Pastel(Color.DarkRed),
                IResponseResult r => $"? {r.Url} - Unknown response..."
            };
    }

    interface IResponseResult {
        string Url { get; }
    }

    readonly struct ResponseError : IResponseResult {
        public string Url { get; }
        public Exception Exception { get; }

        public ResponseError(string url, Exception e) =>
            (Url, Exception) = (url, e);

        public void Deconstruct(out string url, out Exception e) =>
            (url, e) = (Url, Exception);
    }

    readonly struct ResponseTimeResult : IResponseResult {
        public string Url { get; }
        public int StatusCode { get; }
        public long ResponseTime { get; }

        public ResponseTimeResult(string url, int status, long ms ) =>
            (Url, StatusCode, ResponseTime) = (url, status, ms);

        public void Deconstruct(out string url, out int status, out long ms) =>
            (url, status, ms) = (Url, StatusCode, ResponseTime);
    }
}
