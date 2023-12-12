namespace AdventOfCodeLib.Days
{
    public class Day1 : Day
    {
        Dictionary<int, (string, int)> data = [];
        Dictionary<int, (string, int)> dataReverse = [];

        public override string? Description => "Trebuchet?!";

        // Initialize data structures
        public static readonly string[] NumberNameList = ["zero",
            "one",
            "two",
            "three",
            "four",
            "five",
            "six",
            "seven",
            "eight",
            "nine"];

        public override string TestInput1 => @"1abc2
pqr3stu8vwx
a1b2c3d4e5f
treb7uchet";
        public override string TestOutput1 => "142";

        public override string TestInput2 => @"two1nine
eightwothree
abcone2threexyz
xtwone3four
4nineeightseven2
zoneight234
7pqrstsixteen";
        public override string TestOutput2 => "281";

        /// <summary>
        /// Sliding 'hash'. Will keep a unique hash of the 3 last characters only.
        /// </summary>
        /// <param name="hash">Current hash</param>
        /// <param name="input">Next character</param>
        private static int RunHash(int hash, char input)
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
            data = [];
            dataReverse = [];

            for (int i = 0; i < NumberNameList.Length; i++)
            {
                int hash = 0;
                hash = RunHash(hash, NumberNameList[i][0]);
                hash = RunHash(hash, NumberNameList[i][1]);
                hash = RunHash(hash, NumberNameList[i][2]);
                int hashReverse = 0;
                hashReverse = RunHash(hashReverse, NumberNameList[i][2]);
                hashReverse = RunHash(hashReverse, NumberNameList[i][1]);
                hashReverse = RunHash(hashReverse, NumberNameList[i][0]);
                data.Add(hash, (NumberNameList[i][3..], i));
                dataReverse.Add(hashReverse, (NumberNameList[i][3..], i));
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
                        if (input[(i + 1)..].StartsWith(d.Item1))
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
                        if (input[(i + 3)..].StartsWith(d.Item1))
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