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
            List<(int x, int y)> galaxies = GetGalaxies();
            var emptyRows = FindEmptyRows();
            var emptyColumns = FindEmptyColumns();
            long distanceTotal = 0;

            for (int i = 0; i < galaxies.Count; i++)
            {
                for (int j = i + 1; j < galaxies.Count; j++)
                {
                    var distance = GetDistance(emptyRows, emptyColumns, galaxies[i], galaxies[j]);
                    distanceTotal += distance;
                }
            }
            return distanceTotal.ToString();
        }

        public override string RunPart2()
        {
            List<(int x, int y)> galaxies = GetGalaxies();
            var emptyRows = FindEmptyRows();
            var emptyColumns = FindEmptyColumns();
            long distanceTotal = 0;

            for (int i = 0; i < galaxies.Count; i++)
            {
                for (int j = i + 1; j < galaxies.Count; j++)
                {
                    var distance = GetDistance(emptyRows, emptyColumns, galaxies[i], galaxies[j], 1000000);
                    distanceTotal += distance;
                }
            }
            return distanceTotal.ToString();
        }

        private List<(int x, int y)> GetGalaxies()
        {
            var result = new List<(int x, int y)>();

            for (int i = 0; i < Lines.Length; i++)
            {
                for (int j = 0; j < Lines[0].Length; j++)
                {
                    if (Lines[i][j] != '.')
                    {
                        result.Add((j, i));
                    }
                }
            }

            return result;
        }

        private long GetDistance(int[] emptyRows, int[] emptyColumns, (int x, int y) galaxy1, (int x, int y) galaxy2, long factor = 2)
        {
            var (x1, x2) = AOCMath.MinMax(galaxy1.x, galaxy2.x);
            var (y1, y2) = AOCMath.MinMax(galaxy1.y, galaxy2.y);

            var diffX = x2 - x1 + (factor - 1) * (emptyColumns[x2] - emptyColumns[x1]);
            var diffY = y2 - y1 + (factor - 1) * (emptyRows[y2] - emptyRows[y1]);

            return diffX + diffY;
        }

        private int[] FindEmptyRows()
        {
            int[] result = new int[Lines.Length];

            // Outside lines dont matter
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

                if (allEmpty)
                {
                    result[i] = result[i - 1] + 1;
                }
                else
                {
                    result[i] = result[i - 1];
                }
            }

            return result;
        }

        private int[] FindEmptyColumns()
        {
            int[] result = new int[Lines[0].Length];

            // Outside lines dont matter
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

                if (allEmpty)
                {
                    result[j] = result[j - 1] + 1;
                }
                else
                {
                    result[j] = result[j - 1];
                }
            }

            return result;
        }
    }
}