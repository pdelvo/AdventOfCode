using AdventOfCodeLib.Days;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCodeLib
{
    public class HardInstanceProvider : InstanceProvider
    {
        readonly Random random = new(0);
        public Day[] Days => [new Day5()];
        public override string[] GetInstance(int day) => day switch
        {
            //1 => GenerateDay1(),
            5 => GenerateDay5(),
            _ => [],
        };

        private string[] GenerateDay1()
        {
            int lineCount = 1000000;
            var lines = new string[lineCount];

            int lineLengthMin = 500;
            int lineLengthMax = 1000;
            int minNumberOfNumberNames = 5, maxNumberOfNumberNames = 300;

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";

            for (int i = 0; i < lineCount; i++)
            {
                char[] result = new char[random.Next(lineLengthMin, lineLengthMax)];
                for (int j = 0; j < result.Length; j++)
                {
                    result[j] = chars[random.Next(chars.Length)];
                }

                int numberOfNumberNames = random.Next(minNumberOfNumberNames, maxNumberOfNumberNames);

                for (int j = 0; j < numberOfNumberNames; j++)
                {
                    string numberName = Day1.NumberNameList[random.Next(0)];
                    numberName.AsSpan().CopyTo(result.AsSpan()[random.Next(0, result.Length - numberName.Length)..]);
                }

                lines[i] = new string(result);
            }

            return lines;

        }
        private string[] GenerateDay5()
        {
            List<(string vertex, string? left, string? right)> result = [];

            string directionString = GetRandomString(79, 'L', 'R');
            int numberOfCycles = 10;
            Range numberOfEndpointsPerCycleRange = new(1, 10);
            Range firstEndpointRange = new(100, 150);
            Range endpointDistances = new(100, 150);

            for (int i = 0; i < numberOfCycles; i++)
            {
                GenerateCycle(result, directionString, numberOfEndpointsPerCycleRange, firstEndpointRange, endpointDistances);
            }

            return [""];
        }

        private string GetRandomString(int numberOfDigits, params char[] digits)
        {
            StringBuilder sb = new();

            for (int i = 0; i < numberOfDigits; i++)
            {
                sb.Append(digits[random.Next(digits.Length)]);
            }

            return sb.ToString();
        }

        private int GetFreeIndexForDirection(List<(string vertex, string? left, string? right)> list, char direction, bool isLast)
        {
            Func<(string vertex, string? left, string? right), bool> predicate;
            if (direction == 'L')
            {
                predicate = p => p.left == null && ((p.vertex[2] == 'Z') == isLast);
            }
            else
            {
                predicate = p => p.right == null && ((p.vertex[2] == 'Z') == isLast);
            }

            var candidates = list.Count(predicate);

            if (candidates == 0)
            {
                list.Add((GetNewName(list, false, isLast), null, null));

                return list.Count - 1;
            }

            var randomVertex = random.Next(candidates);
            for (int i = 0; i < list.Count; i++)
            {
                if (predicate(list[i]))
                {
                    if (randomVertex-- == 0)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        private List<(string vertex, string? left, string? right)> GenerateCycle(
            List<(string vertex, string? left, string? right)> result, 
            string directionString, 
            Range numberOfEndpointsPerCycleRange, 
            Range firstEndpointRange, 
            Range endpointDistances)
        {

            int numberOfEndpoints = GetRandomFromRange(numberOfEndpointsPerCycleRange);
            int firstEndpoint = GetRandomFromRange(firstEndpointRange);

            int index = 0;
            (string vertex, string? left, string? right) current = (GetNewName(result, isFirst: true), null, null);
            result.Add(current);

            for (int i = 0; i < firstEndpoint - 1; i++)
            {
                var direction = directionString[index++ % directionString.Length];
                var next = result[GetFreeIndexForDirection(result, directionString[direction + 1 % directionString.Length], false)];
            }
            current = result[GetFreeIndexForDirection(result, directionString[index++ % directionString.Length], true)];

            return result;
        }

        private static void SetEntry(List<(string vertex, string? left, string? right)> result, string vertex, string next, char direction)
        {
            for (int i = 0; i < result.Count; i++)
            {
                if (result[i].vertex == vertex)
                {
                    if(direction == 'L')
                    {
                        result[i] = result[i] with
                        {
                            left = next
                        };
                    }
                    else
                    {
                        result[i] = result[i] with
                        {
                            right = next
                        };
                    }
                }
            }
        }

        private string GetNewName(List<(string vertex, string? left, string? right)> usedNames, bool isFirst = false, bool isLast = false)
        {
            char[] allowed = Enumerable.Range('B', 24).Select(x => (char)x).ToArray();
            char[] allowedWithAZ = Enumerable.Range('A', 26).Select(x => (char)x).ToArray();
            if (isFirst && isLast)
            {
                throw new InvalidOperationException("Can't be both first and last");
            }

            string newName = "";
            do
            {
                newName = GetRandomString(2, allowedWithAZ);

                if (isFirst)
                {
                    newName += 'A';
                }
                else if (isLast)
                {
                    newName += 'Z';
                }
                else
                {
                    newName += GetRandomString(1, allowed);
                }
            } while (usedNames.Any(x=> x.vertex == newName));

            return newName;
        }

        private int GetRandomFromRange(Range range)
        {
            return random.Next(range.Start.Value, range.End.Value);
        }
    }
}
