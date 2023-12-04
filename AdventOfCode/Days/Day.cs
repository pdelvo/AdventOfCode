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

        public virtual void Initialize(InstanceDownloader instanceDownloader)
        {
            var dayNumber = int.Parse(GetType().Name.Substring(3));
            Lines = instanceDownloader.GetInstance(dayNumber);
        }

        public abstract int RunPart1();
        public abstract int RunPart2();
    }
}
