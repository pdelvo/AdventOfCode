using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCodeLib.Days
{
    public abstract class Day
    {
        public string[] Lines { get; set; } = new string[0];

        public abstract string TestInput1 { get; }
        public abstract string TestOutput1 { get; }
        public virtual string TestInput2 => TestInput1;
        public abstract string TestOutput2 { get; }

        public int Number { get; set; }
        public string Name { get; set; }
        public virtual string? Description { get; }
        public Day()
        {
            Number  = int.Parse(GetType().Name.Substring(3));
            Name = "Day " + Number;
        }

        public virtual void Initialize(IInstanceProvider instanceDownloader)
        {
            Lines = instanceDownloader.GetInstance(Number);
        }

        public abstract string RunPart1();
        public abstract string RunPart2();

    }
}
