using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace AdventOfCode
{
    public class InstanceDownloader
    {
        HttpClient adventClient;

        Dictionary<int, WeakReference<string[]>> cache = new Dictionary<int, WeakReference<string[]>>();

        int year;

        public InstanceDownloader(int year)
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

            var version = new ProductInfoHeaderValue("AdventOfCodeSupport", "2.4.1");
            var comment = new ProductInfoHeaderValue("(+nuget.org/packages/AdventOfCodeSupport by @Zaneris)");
            adventClient.DefaultRequestHeaders.UserAgent.Add(version);
            adventClient.DefaultRequestHeaders.UserAgent.Add(comment);
            adventClient.DefaultRequestHeaders.Add("cookie", $"session={cookie.Trim()}");
        }

        public string[] GetInstance(int day)
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
