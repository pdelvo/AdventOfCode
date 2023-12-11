using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCodeLib
{
    public class ParsingHelpers
    {

        public static Span<int> ParseNumberList(Span<int> buffer, ReadOnlySpan<char> toParse)
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


        public static Span<long> ParseNumberList(Span<long> buffer, ReadOnlySpan<char> toParse)
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
                buffer[winningNumberBufferCount] = long.Parse(nextNumber);
            }

            return buffer[..winningNumberBufferCount];
        }
    }
}
