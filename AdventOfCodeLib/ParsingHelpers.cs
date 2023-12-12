namespace AdventOfCodeLib
{
    public class ParsingHelpers
    {

        public static Span<int> ParseNumberList(Span<int> buffer, ReadOnlySpan<char> toParse, char splitChar = ' ')
        {
            int winningNumberBufferCount;
            for (winningNumberBufferCount = 0; toParse.Length > 0; winningNumberBufferCount++)
            {
                while (toParse.Length > 0 && toParse[0] == splitChar)
                {
                    toParse = toParse[1..];
                }
                int indexOfWhitespace = toParse.IndexOf(splitChar);
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


        public static Span<long> ParseNumberList(Span<long> buffer, ReadOnlySpan<char> toParse, char splitChar = ' ')
        {
            int winningNumberBufferCount;
            for (winningNumberBufferCount = 0; toParse.Length > 0; winningNumberBufferCount++)
            {
                while (toParse.Length > 0 && toParse[0] == splitChar)
                {
                    toParse = toParse[1..];
                }
                int indexOfWhitespace = toParse.IndexOf(splitChar);
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
