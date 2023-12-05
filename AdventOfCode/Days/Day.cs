using Alba.CsConsoleFormat;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public abstract class Day
    {
        public string[] Lines { get; set; } = new string[0];

        public int Number { get; set; }
        public string Name { get; set; }
        public virtual string? Description { get; }
        public Day()
        {
            Number  = int.Parse(GetType().Name.Substring(3));
            Name = "Day " + Number;
        }

        public virtual void Initialize(InstanceDownloader instanceDownloader)
        {
            Lines = instanceDownloader.GetInstance(Number);
        }

        public abstract int RunPart1();
        public abstract int RunPart2();
    }
}
