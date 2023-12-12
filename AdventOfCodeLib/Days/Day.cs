namespace AdventOfCodeLib.Days
{
    public abstract class Day
    {
        public string[] Lines { get; set; } = [];

        public abstract string TestInput1 { get; }
        public abstract string TestOutput1 { get; }
        public virtual string TestInput2 => TestInput1;
        public abstract string TestOutput2 { get; }

        public int Number { get; set; }
        public string Name { get; set; }
        public virtual string? Description { get; }
        public Day()
        {
            Number  = int.Parse(GetType().Name[3..]);
            Name = "Day " + Number;
        }

        public virtual void Initialize(InstanceProvider instanceDownloader)
        {
            Lines = instanceDownloader.GetInstance(Number);
        }

        public abstract string RunPart1();
        public abstract string RunPart2();

    }
}
