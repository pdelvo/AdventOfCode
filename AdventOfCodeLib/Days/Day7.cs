using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace AdventOfCodeLib.Days
{
    public class Day7 : Day
    {
        public override string? Description => "Camel Cards";
        public override string TestInput1 => @"32T3K 765
T55J5 684
KK677 28
KTJJT 220
QQQJA 483";
        public override string TestOutput1 => "6440";
        public override string TestOutput2 => "5905";

        private static bool JokerRule = false;
        public override string RunPart1()
        {
            JokerRule = false;
            return Run();
        }

        public override string RunPart2()
        {
            JokerRule = true;
            return Run();
        }

        private string Run()
        {
            var hands = ParseValues();
            Array.Sort(hands);

            long result = 0;

            for (int i = 0; i < hands.Length; i++)
            {
                result += (i + 1) * hands[hands.Length - i - 1].Value;
            }

            return result.ToString();
        }

        private Hand[] ParseValues()
        {
            var hands = new Hand[Lines.Length];
            for (int i = 0; i < Lines.Length; i++)
            {
                hands[i] = new Hand(Lines[i], int.Parse(Lines[i].AsSpan()[6..]));
            }

            return hands;
        }

        unsafe struct Hand : IComparable<Hand>
        {
            public string Cards { get; set; }
            public int Value { get; set; }

            public fixed byte NumberCounter[6];
            //public byte[] NumberCounter;

            public Hand(string card, int value)
            {
                Cards = card;
                Value = value;

                //NumberCounter = new byte[6];

                SetNumberCounter();
            }

            private void SetNumberCounter()
            {
                Span<int> counter = stackalloc int[13];

                for (int i = 0; i < 5; i++)
                {
                    char c = Cards[i];
                    counter[CharToPosition(c)]++;
                }

                int maxIndex = 0;
                int maxCount = 0;
                for (int i = 0; i < counter.Length; i++)
                {
                    if (!JokerRule || i != 3)
                    {
                        NumberCounter[counter[i]]++;
                        if (maxCount < counter[i])
                        {
                            maxCount = counter[i];
                            maxIndex = i;
                        }
                    }
                }

                if (JokerRule)
                {
                    NumberCounter[counter[maxIndex]]--;
                    NumberCounter[counter[maxIndex] + counter[3]]++;
                }
            }

            private static int CharToPosition(char c) => c switch
            {
                'A' => 0,
                'K' => 1,
                'Q' => 2,
                'J' => 3,
                'T' => 4,
                '9' => 5,
                '8' => 6,
                '7' => 7,
                '6' => 8,
                '5' => 9,
                '4' => 10,
                '3' => 11,
                '2' => 12,
                _ => -1,
            };

            private static int CharToSinglePriority(char c)
            {
                if (JokerRule && c == 'J')
                {
                    return 20;
                }
                else
                {
                    return CharToPosition(c);
                }
            }

            public int CompareTo(Hand other)
            {
                int compareResult = GetResultPriority().CompareTo(other.GetResultPriority());
                for (int i = 0; i <= 5 && compareResult == 0; i++)
                {
                    compareResult = CharToSinglePriority(Cards[i]).CompareTo(CharToSinglePriority(other.Cards[i]));
                }

                return compareResult;
            }

            private int GetResultPriority()
            {
                if (NumberCounter[5] == 1)
                {
                    return 0;
                }
                else if (NumberCounter[4] == 1)
                {
                    return 1;
                }
                else if (NumberCounter[3] == 1 && NumberCounter[2] == 1)
                {
                    return 2;
                }
                else if (NumberCounter[3] == 1)
                {
                    return 3;
                }
                else if (NumberCounter[2] == 2)
                {
                    return 4;
                }
                else if (NumberCounter[2] == 1)
                {
                    return 5;
                }
                return 6;
            }
        }
    }
}