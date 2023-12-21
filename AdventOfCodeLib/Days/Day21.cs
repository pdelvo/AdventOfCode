
using System;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

namespace AdventOfCodeLib.Days
{
    public class Day21 : Day
    {
        public override string? Description => "Step Counter";
        public override string TestInput1 => @"...........
.....###.#.
.###.##..#.
..#.#...#..
....#.#....
.##..S####.
.##..#...#.
.......##..
.##.#.####.
.##..##.##.
...........";
        public override string TestOutput1 => "16";

        public override string TestOutput2 => "0";
        public override string RunPart1()
        {
            int maxSteps = Lines.Length == 11 ? 6 : 64;

            return CountSteps(maxSteps, FindS()).ToString();
        }
        public override string RunPart2()
        {
            int maxSteps = 26501365;
            if (Lines.Length == 11)
            {
                return "";
            }
            int dimension = Lines.Length;

            // We cant just count all cells because of parity
            int numberOfEmptyCellsA = CountSteps(2 * dimension + (maxSteps % 2), (0, 0));
            int numberOfEmptyCellsB = CountSteps(2 * dimension + (maxSteps % 2 + 1), (0, 0));

            // Horizontal and Vertical Axis

            var S = FindS();
            int fromCenterBorder = maxSteps;

            // Number of full cells in a direction
            long full = fromCenterBorder / dimension - 1;

            long totalCellCount = 0;
            // This is 130, the exact size. So only one partial one
            int axisRemaining = (int)(maxSteps - full * dimension - S.x - 1);
            totalCellCount += CountSteps(axisRemaining, (0, S.y));
            totalCellCount += CountSteps(axisRemaining, (dimension - 1, S.y));
            totalCellCount += CountSteps(axisRemaining, (S.x, 0));
            totalCellCount += CountSteps(axisRemaining, (S.x, dimension - 1));

            // Figure out other partial cells
            axisRemaining -= S.x + 1;
            int partialPP = CountSteps(axisRemaining, (0, 0));
            int partialPM = CountSteps(axisRemaining, (0, dimension - 1));
            int partialMP = CountSteps(axisRemaining, (dimension - 1, 0));
            int partialMM = CountSteps(axisRemaining, (dimension - 1, dimension - 1));
            axisRemaining += dimension;
            int partialPPB = CountSteps(axisRemaining, (0, 0));
            int partialPMB = CountSteps(axisRemaining, (0, dimension - 1));
            int partialMPB = CountSteps(axisRemaining, (dimension - 1, 0));
            int partialMMB = CountSteps(axisRemaining, (dimension - 1, dimension - 1));

            // full + 2 is the cell that is partial.
            // (full + 2) + 1 is the count in the diagonal
            // ((full + 2) + 1) - 2 to subtract the axis ones we counted separately. So we could have just used "full".
            var diagonalCountA = ((full + 2) + 1) - 2;
            var diagonalCountB = diagonalCountA - 1;

            totalCellCount += diagonalCountA * (partialPP + partialPM + partialMP + partialMM);
            totalCellCount += diagonalCountB * (partialPPB + partialPMB + partialMPB + partialMMB);

            // Count in the inner cells
            // Count all (x, y) with |x| + |y| <= full
            // 2* (1 + 3 + 5 + 7 ... + (2D + 1)) - (2D + 1) = 4*D^2 + 4*D + 1
            // 2*(D + 1)^2 -2D - 1
            // Unfortunately the grid is odd width so the parity changes

            // To count only the ones with the same parity as S
            // 2*(1 + 2 + ... + 2*(D/2)) + 2*(D/2) + 1
            // = 2*((N + 1)*N/2) + N + 1 where N = 2*(D/2)
            // = (N + 1)*N + N + 1 where N = 2*(D/2)
            // = (N + 1)*(N + 1) where N = 2*(D/2)
            long N = 2 * (full / 2);
            var numParityA = (N + 1) * (N + 1);
            var numParityB = (2 * (full + 1) * (full + 1) - 2 * full - 1) - numParityA;
            // Make sure we counted everything
            Debug.Assert(numParityA +  numParityB == (2 * (full + 1) * (full + 1) - 2 * full - 1));
            totalCellCount += numParityA * numberOfEmptyCellsA;
            totalCellCount += numParityB * numberOfEmptyCellsB;

            return totalCellCount.ToString();
        }

        private int CountSteps(int maxSteps, (int x, int y) start, bool partB = false)
        {
            var S = start;

            HashSet<(int x, int y)> canVisit = new HashSet<(int x, int y)>();
            canVisit.Add(S);

            Queue<(int x, int y)> currentRound= new();
            Queue<(int x, int y)> nextRound= new();

            nextRound.Enqueue(S);

            for (int i = 0; i < maxSteps; i++)
            {
                (currentRound, nextRound) = (nextRound, currentRound);

                nextRound.Clear();

                while (currentRound.TryDequeue(out var result))
                {
                    var (currentX, currentY) = result;
                    AddIfValid(canVisit, nextRound, currentX - 1, currentY, partB);
                    AddIfValid(canVisit, nextRound, currentX + 1, currentY, partB);
                    AddIfValid(canVisit, nextRound, currentX, currentY - 1, partB);
                    AddIfValid(canVisit, nextRound, currentX, currentY + 1, partB);
                }
            }
            int counter = 0;

            foreach (var item in canVisit)
            {
                if ((item.x + item.y) % 2 == (S.x + S.y + maxSteps) % 2)
                {
                    counter++;
                }
            }

            return counter;
        }

        private void AddIfValid(HashSet<(int x, int y)> canVisit, Queue<(int x, int y)> nextRound, int x, int y, bool partB = false)
        {
            if ((partB || IsValid(x, y)) && Lines[GetInRange(y, Lines.Length)][GetInRange(x, Lines[0].Length)] != '#')
            {
                if (canVisit.Add((x, y)))
                {
                    nextRound.Enqueue((x, y));
                }
            }
        }

        private int GetInRange(int value, int limit)
        {
            return ((value % limit) + limit) % limit;
        }

        private (int x, int y) FindS()
        {
            for (int i = 0; i < Lines.Length; i++)
            {
                for (int j = 0; j < Lines[0].Length; j++)
                {
                    if (Lines[i][j] == 'S')
                    {
                        return (j, i);
                    }
                }
            }
            return (-1, -1);
        }

        public bool IsValid(int x, int y)
            => x >= 0 && y >= 0 && x < Lines[0].Length && y < Lines.Length;

    }
}