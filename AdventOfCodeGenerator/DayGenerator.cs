﻿using Microsoft.CodeAnalysis;

using System.Linq;

namespace AdventOfCodeGenerator
{
    [Generator]
    public class DayGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            string[] days = context.Compilation.GetSymbolsWithName(x => x.StartsWith("Day"), SymbolFilter.Type).Where(x => !x.IsAbstract).Select(x=>x.Name).ToArray();

            string daysText = string.Join(", ", days.Select(x => "new " + x + "()"));

            // Build up the source code
            string source = $@"// <auto-generated/>
using System;
using AdventOfCodeLib.Days;

namespace AdventOfCodeLib
{{
    public partial interface IInstanceProvider
    {{
        public Day[] Days => [{daysText}];
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
