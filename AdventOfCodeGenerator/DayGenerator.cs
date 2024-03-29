﻿using Microsoft.CodeAnalysis;

using System;
using System.Linq;

namespace AdventOfCodeGenerator
{
    [Generator]
    public class DayGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            string[] days = context.Compilation.GetSymbolsWithName(x => x.StartsWith("Day"), SymbolFilter.Type).Where(x => !x.IsAbstract).Select(x=>x.Name).ToArray();
            Array.Sort(days, (a, b) => int.Parse(a.Substring(3)).CompareTo(int.Parse(b.Substring(3))));
            string daysText = string.Join(", ", days.Select(x => "new " + x + "()"));

            // Build up the source code
            string source = $@"// <auto-generated/>
using System;
using AdventOfCodeLib.Days;

namespace AdventOfCodeLib
{{
    public abstract partial class InstanceProvider
    {{
        public virtual Day[] Days {{ get; }} = [{daysText}];
    }}
}}
";

            // Add the source code to the compilation
            context.AddSource($"IInstanceProvider.g.cs", source);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }
    }
}
