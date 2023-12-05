using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace AdventOfCode.Days
{
    public class Day4 : Day
    {
        public override string? Description => "Scratchcards";

        public override string RunPart1()
        {
            Span<int> winningNumberBuffer = stackalloc int[100];
            Span<int> numberBuffer = stackalloc int[100];
            int sum = 0;

            var lines = Lines;

            for (int i = 0; i < lines.Length; i++)
            {
                int colon = lines[i].IndexOf(':');
                int pipe = lines[i].IndexOf('|');
                var winningNumberRemaining = lines[i].AsSpan()[(colon + 1)..pipe];
                var numberRemaining = lines[i].AsSpan()[(pipe + 1)..];
                var winningNumbers = Parse(winningNumberBuffer, winningNumberRemaining);
                var numbers = Parse(numberBuffer,  numberRemaining);

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
            return sum.ToString();
        }

        public override string RunPart2()
        {
            Span<int> winningNumberBuffer = new int[100];
            Span<int> numberBuffer = new int[100];

            int sum = 0;

            var lines = Lines;

            int[] counts = new int[lines.Length];

            for (int i = 0; i < counts.Length; i++)
            {
                counts[i] = 1;
            }

            for (int i = 0; i < lines.Length; i++)
            {
                int colon = lines[i].IndexOf(':');
                int pipe = lines[i].IndexOf('|', colon);
                var winningNumberRemaining = lines[i].AsSpan()[(colon + 1)..pipe];
                var numberRemaining = lines[i].AsSpan()[(pipe + 1)..];
                var winningNumbers = Parse(winningNumberBuffer, winningNumberRemaining);
                var numbers = Parse(numberBuffer, numberRemaining);

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

                sum += counts[i];
            }

            return sum.ToString();
        }

        private static ReadOnlySpan<int> Parse(Span<int> buffer, ReadOnlySpan<char> toParse)
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

            return buffer[..winningNumberBufferCount];
        }
    }
}