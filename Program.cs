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
                    TimingMessage(
                        await GetTimedResponse(siteurl)));
            }
        }

        async static IAsyncEnumerable<string> GetSiteUrls(TextReader urlSource) {
            string siteurl;
            while(true) {
                siteurl = await urlSource.ReadLineAsync();
                if(siteurl != null) {
                    yield return siteurl;
                }
                else {
                    yield break;
                }
            }
        }

        static async Task<ResponseTimeResult> GetTimedResponse(string url) {
            using var http = new HttpClient();
            ResponseTimeResult result;
            try {
                Stopwatch watch = Stopwatch.StartNew();
                var response = await http.GetAsync(url);
                result = ResponseTimeResult.Response(
                    url,
                    (int)response.StatusCode,
                    watch.ElapsedMilliseconds
                );
            }
            catch (Exception e) {
                result = ResponseTimeResult.Error(url, e);
            }
            return result;
        }

        static string TimingMessage(ResponseTimeResult result) {
            if(result.Exception != null) {
                return $"x {result.Url} Request failed miserably: {result.Exception.Message}"
                    .Pastel(Color.Red);
            } 

            if(result.StatusCode == 200) {
                switch(result.ResponseTime) {
                    case var ms when ms < 300:
                        return $"- {result.Url} Excellent response time: {ms}"
                            .Pastel(Color.LightBlue);
                    case var ms when ms < 1000:
                        return $"- {result.Url} Good response time: {ms}"
                            .Pastel(Color.YellowGreen);
                    case var ms when ms < 3000:
                        return $"- {result.Url} Decent response time: {ms}"
                            .Pastel(Color.Yellow);
                    case var ms when ms >= 3000:
                        return $"- {result.Url} Bad response time: {ms}"
                            .Pastel(Color.Orange);
                }
            } else if (result.StatusCode == 404) {
                return $"- {result.Url} Not found url".Pastel(Color.DarkOrange);
            } else if (result.StatusCode == 500) {
                return $"- {result.Url} Server error".Pastel(Color.DarkOrange);
            }

            return "Whut?";
        }
    }

    readonly struct ResponseTimeResult {
        public string Url { get; }
        public int StatusCode { get; }
        public long ResponseTime { get; }
        public Exception? Exception { get; }

        public ResponseTimeResult(string url, int status, long ms, Exception? error) =>
            (Url, StatusCode, ResponseTime, Exception) = (url, status, ms, error);

        public static ResponseTimeResult Response(string url, int status, long ms) =>
            new ResponseTimeResult(url, status, ms, null);

        public static ResponseTimeResult Error(string url, Exception error) =>
            new ResponseTimeResult(url, 0, 0, error);
    }
}
