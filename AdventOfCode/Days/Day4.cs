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
            Span<int> winningNumberBuffer = stackalloc int[50];
            Span<int> numberBuffer = stackalloc int[50];
            int sum = 0;

            var lines = Lines;

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].AsSpan();
                int colon = line.IndexOf(':');
                var winningNumberRemaining = line[(colon + 1)..];
                int pipe = winningNumberRemaining.IndexOf('|');
                var numberRemaining = winningNumberRemaining[(pipe + 1)..];
                winningNumberRemaining = winningNumberRemaining[..pipe];
                var winningNumbers = Tools.ParseNumberList(winningNumberBuffer, winningNumberRemaining);
                var numbers = Tools.ParseNumberList(numberBuffer, numberRemaining);

                int numberOfWins = 0;
                // Alternative way which is slower
                //winningNumbers.Sort();
                //numbers.Sort();

                //int winningNumberIndex = 0;
                //int numberIndex = 0;
                //while  (winningNumberIndex < winningNumbers.Length && numberIndex < numbers.Length)
                //{
                //    int number = numbers[numberIndex];
                //    int winningNumber = winningNumbers[winningNumberIndex];

                //    if (number == winningNumber)
                //    {
                //        numberOfWins++;
                //        numberIndex++;
                //        winningNumberIndex++;
                //    }
                //    else if (number < winningNumber)
                //    {
                //        numberIndex++;
                //    }
                //    else
                //    {
                //        winningNumberIndex++;
                //    }
                //}
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
            Span<int> winningNumberBuffer = new int[50];
            Span<int> numberBuffer = new int[50];

            int sum = 0;

            var lines = Lines;

            int[] counts = new int[lines.Length];

            for (int i = 0; i < counts.Length; i++)
            {
                counts[i] = 1;
            }

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].AsSpan();
                int colon = line.IndexOf(':');
                var winningNumberRemaining = line[(colon + 1)..];
                int pipe = winningNumberRemaining.IndexOf('|');
                var numberRemaining = winningNumberRemaining[(pipe + 1)..];
                winningNumberRemaining = winningNumberRemaining[..pipe];
                var winningNumbers = Tools.ParseNumberList(winningNumberBuffer, winningNumberRemaining);
                var numbers = Tools.ParseNumberList(numberBuffer, numberRemaining);

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
    }
}