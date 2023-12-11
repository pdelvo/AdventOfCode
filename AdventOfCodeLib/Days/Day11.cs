using System.Collections.Generic;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace AdventOfCodeLib.Days
{
    public class Day11 : Day
    {
        public override string? Description => "Cosmic Expansion";
        public override string TestInput1 => @"...#......
.......#..
#.........
..........
......#...
.#........
.........#
..........
.......#..
#...#.....";
        public override string TestOutput1 => "374";

        public override string TestOutput2 => "82000210";
        public override string RunPart1()
        {
            long distanceTotal = GetTotal(2);
            return distanceTotal.ToString();
        }

        public override string RunPart2()
        {
            long distanceTotal = GetTotal(1_000_000);
            return distanceTotal.ToString();
        }

        private long GetTotal(long factor)
        {
            (int[] rowCounter, int[] columnCounter) = GetGalaxies();
            var emptyRows = FindEmptyRows();
            var emptyColumns = FindEmptyColumns();
            long distanceTotal = 0;

            int last = 0;
            for (int i = 1; i < rowCounter.Length; i++)
            {
                if (rowCounter[i] == rowCounter[last])
                    continue;

                var distance = GetDistance(emptyRows, last, i, factor);
                distanceTotal += rowCounter[last] * (rowCounter[rowCounter.Length - 1] - rowCounter[last]) * distance;

                last = i;
            }
            last = 0;
            for (int i = 1; i < columnCounter.Length; i++)
            {
                if (columnCounter[i] == columnCounter[last])
                    continue;

                var distance = GetDistance(emptyColumns, last, i, factor);
                distanceTotal += columnCounter[last] * (columnCounter[columnCounter.Length - 1] - columnCounter[last]) * distance;

                last = i;
            }

            return distanceTotal;
        }

        private (int[] rowCounter, int[] columnCounter) GetGalaxies()
        {
            int[] rowCounter = new int[Lines.Length];
            int[] columnCounter = new int[Lines[0].Length];

            for (int i = 0; i < Lines.Length; i++)
            {
                for (int j = 0; j < Lines[0].Length; j++)
                {
                    if (Lines[i][j] != '.')
                    {
                        rowCounter[i]++;
                        columnCounter[j]++;
                    }
                }
            }

            for (int i = 1; i < rowCounter.Length; i++)
            {
                rowCounter[i] += rowCounter[i - 1];
            }

            for (int j = 1; j < columnCounter.Length; j++)
            {
                columnCounter[j] += columnCounter[j - 1];
            }

            return (rowCounter, columnCounter);
        }

        private long GetDistance(int[] counterArray, int i1, int i2, long factor = 2)
            => i2 - i1 + (factor - 1) * (counterArray[i2] - counterArray[i1]);

        private int[] FindEmptyRows()
        {
            int[] result = new int[Lines.Length];

            // Outside lines don't matter
            for (int i = 1; i < Lines.Length; i++)
            {
                bool allEmpty = true;
                for (int j = 0; j < Lines[0].Length; j++)
                {
                    if (Lines[i][j] != '.')
                    {
                        allEmpty = false;
                        break;
                    }
                }

                result[i] = result[i - 1] + (allEmpty ? 1 : 0);
            }

            return result;
        }

        private int[] FindEmptyColumns()
        {
            int[] result = new int[Lines[0].Length];

            // Outside lines don't matter
            for (int j = 1; j < Lines[0].Length; j++)
            {
                bool allEmpty = true;
                for (int i = 0; i < Lines.Length; i++)
                {
                    if (Lines[i][j] != '.')
                    {
                        allEmpty = false;
                        break;
                    }
                }

                result[j] = result[j - 1] + (allEmpty ?  1 : 0);
            }

            return result;
        }
    }
}