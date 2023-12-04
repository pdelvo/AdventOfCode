using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace AdventOfCode.Days
{
    public class Day4 : IDay
    {
        string[] lines;
        public Day4()
        {
            lines = File.ReadAllLines("Input/input4.txt");
        }

        public void RunPart1()
        {
            int[] winningNumberBuffer = new int[100];
            int[] numberBuffer = new int[100];
            int sum = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                int colon = lines[i].IndexOf(':');
                int pipe = lines[i].IndexOf('|');
                var winningNumberRemaining = ((ReadOnlySpan<char>)lines[i])[(colon + 1)..pipe];
                var numberRemaining = ((ReadOnlySpan<char>)lines[i])[(pipe + 1)..];
                var winningNumbers = Parse(winningNumberBuffer, ref winningNumberRemaining);
                var numbers = Parse(numberBuffer, ref numberRemaining);

                int numberOfWins = 0;
                for (int x = 0; x < numbers.Length; x++)
                {
                    for (int y = 0; y < winningNumbers.Length; y++)
                    {
                        if (numbers[x] == winningNumbers[y])
                        {
                            numberOfWins++;
                            break;
                        }
                    }
                }
                if (numberOfWins > 0)
                {
                    sum += 1 << (numberOfWins - 1);
                }
            }
            Console.WriteLine("Day 4 Part 1: " + sum);
        }

        public void RunPart2()
        {
            int[] winningNumberBuffer = new int[100];
            int[] numberBuffer = new int[100];
            int[] counts = new int[lines.Length];

            for (int i = 0; i < counts.Length; i++)
            {
                counts[i] = 1;
            }

            for (int i = 0; i < lines.Length; i++)
            {
                int colon = lines[i].IndexOf(':');
                int pipe = lines[i].IndexOf('|');
                var winningNumberRemaining = ((ReadOnlySpan<char>)lines[i])[(colon + 1)..pipe];
                var numberRemaining = ((ReadOnlySpan<char>)lines[i])[(pipe + 1)..];
                var winningNumbers = Parse(winningNumberBuffer, ref winningNumberRemaining);
                var numbers = Parse(numberBuffer, ref numberRemaining);

                int numberOfWins = 0;
                for (int x = 0; x < numbers.Length; x++)
                {
                    for (int y = 0; y < winningNumbers.Length; y++)
                    {
                        if (numbers[x] == winningNumbers[y])
                        {
                            numberOfWins++;
                            break;
                        }
                    }
                }

                for (int j = 0; j < numberOfWins; j++)
                {
                    counts[i + j + 1] += counts[i];
                }
            }
            //Console.WriteLine("Day 4 Part 2: " + counts.Sum());
        }

        private static ReadOnlySpan<int> Parse(int[] buffer, ref ReadOnlySpan<char> toParse)
        {
            int winningNumberBufferCount;
            for (winningNumberBufferCount = 0; toParse.Length > 0; winningNumberBufferCount++)
            {
                while (toParse.Length > 0 && toParse[0] == ' ')
                {
                    toParse = toParse[1..];
                }
                int indexOfWhitespace = toParse.IndexOf(' ');
                ReadOnlySpan<char> nextNumber = toParse;
                if (indexOfWhitespace >= 0)
                {
                    nextNumber = toParse[0..indexOfWhitespace];
                    toParse = toParse[(indexOfWhitespace + 1)..];
                }
                else
                {
                    toParse = toParse[nextNumber.Length..];
                }
                buffer[winningNumberBufferCount] = int.Parse(nextNumber);
            }

            return ((ReadOnlySpan<int>)buffer)[..winningNumberBufferCount];
        }
    }
}