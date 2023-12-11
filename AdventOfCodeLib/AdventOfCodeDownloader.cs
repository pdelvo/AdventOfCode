using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace AdventOfCodeLib
{
    public class AdventOfCodeDownloader : InstanceProvider
    {
        readonly HttpClient adventClient;

        readonly Dictionary<int, WeakReference<string[]>> cache = [];

        readonly int year;

        public AdventOfCodeDownloader(int year)
        {
            this.year = year;
            var builder = new ConfigurationBuilder();
            builder.AddUserSecrets(Assembly.GetEntryAssembly()!);
            var config = builder.Build();
            var cookie = config["session"];
            if (string.IsNullOrWhiteSpace(cookie))
                throw new Exception("User secret could not be found");

            var handler = new HttpClientHandler { UseCookies = false };
            adventClient = new HttpClient(handler) { BaseAddress = new Uri("https://adventofcode.com/") };

            var version = new ProductInfoHeaderValue("ddelvo_custom_instance_downloader", "1.0.0");
            adventClient.DefaultRequestHeaders.UserAgent.Add(version);
            adventClient.DefaultRequestHeaders.Add("cookie", $"session={cookie.Trim()}");
        }

        public override string[] GetInstance(int day)
        {
            string path = $"input/input{day}.txt";
            if (cache.ContainsKey(day))
            {
                var weakReference = cache[day];

                if (weakReference.TryGetTarget(out var target))
                {
                    return target;
                }
            }

            try
            {
                var lines = File.ReadAllLines(path);

                cache[day] = new WeakReference<string[]>(lines);

                return lines;
            }
            catch (Exception)
            {
                var response = adventClient.GetAsync($"{year}/day/{day}/input").Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Input download {year}-{day} failed. {response.ReasonPhrase}");
                }

                var str = response.Content.ReadAsStringAsync().Result;

                Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                File.WriteAllText(path, str);

                Thread.Sleep(5000);

                return GetInstance(day);
            }
        }
    }
}
