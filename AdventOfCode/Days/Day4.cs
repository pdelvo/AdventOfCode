using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace AdventOfCode.Days
{
    public class Day4 : IDay
    {
        Regex regex;
        string[] lines;
        int[] counts;
        public Day4()
        {
            regex = new Regex("^Card (?<cardNumber>[0-9 ]+): (?<winningNumbers>[^|]+) \\| (?<numbers>[0-9 ]+)$", RegexOptions.Compiled);
            lines = File.ReadAllLines("Input/input4.txt");
            counts = new int[lines.Length];
        }

        public void RunPart1()
        {
            int sum = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                int colon = lines[i].IndexOf(':');
                int pipe = lines[i].IndexOf('|');
                var winningNumbers = lines[i][(colon + 1)..pipe].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();
                var numbers = lines[i][(pipe + 1)..].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();

                int numberOfWins = winningNumbers.Intersect(numbers).Count();
                if (numberOfWins > 0)
                {
                    sum += 1 << (numberOfWins - 1);
                }
            }
            Console.WriteLine("Day 4 Part 1: " + sum);
        }

        public void RunPart2()
        {
            for (int i = 0; i < counts.Length; i++)
            {
                counts[i] = 1;
            }

            for (int i = 0; i < lines.Length; i++)
            {
                //var match = regex.Match(lines[i]);

                int colon = lines[i].IndexOf(':');
                int pipe = lines[i].IndexOf('|');
                var winningNumbers = lines[i][(colon + 1)..pipe].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();
                var numbers = lines[i][(pipe + 1)..].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();

                int numberOfWins = winningNumbers.Intersect(numbers).Count();

                for (int j = 0; j < numberOfWins; j++)
                {
                    counts[i + j + 1] += counts[i];
                }
            }
            Console.WriteLine("Day 4 Part 1: " + counts.Sum());
        }
    }
}