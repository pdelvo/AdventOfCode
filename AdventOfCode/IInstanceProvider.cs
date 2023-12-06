using AdventOfCode.Days;

namespace AdventOfCode
{
    public interface IInstanceProvider
    {
        string[] GetInstance(int day);

        public Day[] Days => Program.Days;
    }
}