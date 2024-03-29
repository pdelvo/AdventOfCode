﻿namespace AdventOfCodeLib
{
    public static class AOCMath
    {
        public static long ChineseRemainder((long divisor, long remainder)[] input)
        {
            throw new NotImplementedException();
        }

        public static bool MakeCoprime(List<(long divisor, long remainder)> input)
        {
            for (int i = 0; i < input.Count; i++)
            {
                var (divisor1, remainder1) = input[i];
                for (int j = i + 1; j < input.Count; j++)
                {
                    var (divisor2, remainder2) = input[j];

                    var gcd = GreatestCommonDivisor(divisor1, divisor2);

                    if (gcd == 1)
                    {
                        continue;
                    }
                    else
                    {
                        if ((remainder1 - remainder2) % gcd != 0)
                        {
                            // System is not solvable
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public static long LeastCommonMultiple(long a, long b)
        {
            return a * (b / GreatestCommonDivisor(a, b));
        }

        public static (long min, long max) MinMax(long a, long b)
        {
            if (a < b)
            {
                return (a, b);
            }

            return (b, a);
        }

        public static long GreatestCommonDivisor(long a, long b)
        {
            while (true)
            {
                (a, b) = MinMax(a, b);

                if (a == 0)
                {
                    return b;
                }

                b %= a;
            }
        }

        public static (long gcd, long u, long v) GreatestCommonDivisorExtended(long a, long b)
        {
            long rn = a;
            long sn = 1;
            long tn = 0;
            long rn1 = b;
            long sn1 = 0;
            long tn1 = 1;

            while (true)
            {
                long q = rn / rn1;

                long rn2 = rn % rn1;
                long sn2 = sn - q * sn1;
                long tn2 = tn - q * tn1;

                if (rn2 == 0)
                {
                    return (rn1, sn1, tn1);
                }

                rn = rn1;
                sn = sn1;
                tn = tn1;

                rn1 = rn2;
                sn1 = sn2;
                tn1 = tn2;
            }
        }

        public static int Sum(this ReadOnlySpan<int> input)
        {
            int sum = 0;

            for (int i = 0; i < input.Length; i++)
            {
                sum += input[i];
            }

            return sum;
        }

        /// <summary>
        /// Number of partitions of _n_ with exactly k (non-empty) parts
        /// </summary>
        /// <param name="n">Size of the ground set</param>
        /// <param name="k">Number of groups</param>
        public static long StirlingNumber2ndKind(int n, int k)
        {
            if (n == k)
            {
                return 1;
            }
            if (k == 0 || k > n)
            {
                return 0;
            }

            //Memoization might be good here
            return StirlingNumber2ndKind(n - 1, k - 1) + k * StirlingNumber2ndKind(n - 1, k);
        }
    }
}
