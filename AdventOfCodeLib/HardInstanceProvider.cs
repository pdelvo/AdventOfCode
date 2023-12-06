using AdventOfCodeLib.Days;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCodeLib
{
    public class HardInstanceProvider : IInstanceProvider
    {
        Random random = new Random(0);
        public Day[] Days => [new Day1()];
        public string[] GetInstance(int day)
        {
            switch (day)
            {
                case 1:
                    return GenerateDay1();
            }

            return null;
        }

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
                    numberName.AsSpan().CopyTo(result[random.Next(0, result.Length - numberName.Length)..]);
                }

                lines[i] = new string(result);
            }

            return lines;

        }
    }
}
