using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace AdventOfCode.Days
{
    public class Day1 : Day
    {
        Dictionary<int, (string, int)> data = new Dictionary<int, (string, int)>();
        Dictionary<int, (string, int)> dataReverse = new Dictionary<int, (string, int)>();

        public override string? Description => "Trebuchet?!";
        public override void Initialize(InstanceDownloader downloader)
        {
            base.Initialize(downloader);
        }

        private int RunHash(int hash, char input)
        {
            return ((hash << 8) | (byte)input) & 0xffffff;
        }

        public override string RunPart1()
        {
            int sum = 0;

            foreach (var line in Lines)
            {
                int lineValue = 0;

                for (int i = 0; i < line.Length; i++)
                {
                    if (char.IsDigit(line[i]))
                    {
                        lineValue += 10 * (line[i] - '0');
                        break;
                    }
                }

                for (int i = line.Length - 1; i >= 0; i--)
                {
                    if (char.IsDigit(line[i]))
                    {
                        lineValue += line[i] - '0';
                        break;
                    }
                }

                sum += lineValue;
            }

            return sum.ToString();
        }

        public override string RunPart2()
        {
            string[] map = ["zero",
                "one",
                "two",
                "three",
                "four",
                "five",
                "six",
                "seven",
                "eight",
                "nine"];
            data = new Dictionary<int, (string, int)>();
            dataReverse = new Dictionary<int, (string, int)>();

            for (int i = 0; i < map.Length; i++)
            {
                int hash = 0;
                hash = RunHash(hash, map[i][0]);
                hash = RunHash(hash, map[i][1]);
                hash = RunHash(hash, map[i][2]);
                int hashReverse = 0;
                hashReverse = RunHash(hash, map[i][2]);
                hashReverse = RunHash(hash, map[i][1]);
                hashReverse = RunHash(hash, map[i][0]);
                data.Add(hash, (map[i], i));
                dataReverse.Add(hashReverse, (map[i], i));
            }
            int sum = 0;
            foreach (var line in Lines)
            {
                var left = GetLeftValue(line);
                var right = GetRightValue(line);

                sum += 10 * left + right;
            }

            return sum.ToString();
        }

        int GetLeftValue(ReadOnlySpan<char> input)
        {
            int hash = 0;
            for(int i = 0; i < input.Length; i++)
            {
                if (char.IsDigit(input[i]))
                {
                    return input[i] - '0';
                }
                else
                {
                    hash = RunHash(hash, input[i]);

                    if (data.TryGetValue(hash, out var d))
                    {
                        if (input[(i - 2)..].StartsWith(d.Item1))
                        {
                            return d.Item2;
                        }
                    }
                }
            }

            return -1;
        }

        int GetRightValue(ReadOnlySpan<char> input)
        {
            int hash = 0;
            for (int i = input.Length - 1; i >= 0; i--)
            {
                if (char.IsDigit(input[i]))
                {
                    return input[i] - '0';
                }
                else
                {
                    hash = RunHash(hash, input[i]);

                    if (dataReverse.TryGetValue(hash, out var d))
                    {
                        if (input[i..].StartsWith(d.Item1))
                        {
                            return d.Item2;
                        }
                    }
                }
            }

            return -1;
        }
    }
}