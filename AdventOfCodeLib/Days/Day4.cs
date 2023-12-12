namespace AdventOfCodeLib.Days
{
    public class Day4 : Day
    {
        public override string? Description => "Scratchcards";
        public override string TestInput1 => @"Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11";
        public override string TestOutput1 => "13";
        public override string TestOutput2 => "30";

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
                var winningNumbers = ParsingHelpers.ParseNumberList(winningNumberBuffer, winningNumberRemaining);
                var numbers = ParsingHelpers.ParseNumberList(numberBuffer, numberRemaining);

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

            long sum = 0;

            var lines = Lines;

            long[] counts = new long[lines.Length];

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
                var winningNumbers = ParsingHelpers.ParseNumberList(winningNumberBuffer, winningNumberRemaining);
                var numbers = ParsingHelpers.ParseNumberList(numberBuffer, numberRemaining);

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
                    if (i + j + 1 < counts.Length)
                    {
                        counts[i + j + 1] += counts[i];
                    }
                    else
                    {
                        break;
                    }
                }

                sum += counts[i];
                counts[i] = 0;
            }

            return sum.ToString();
        }
    }
}