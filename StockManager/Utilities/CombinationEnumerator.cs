using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace StockManager.Utilities
{
    class CombinationEnumerator
    {
        private Random dice = new Random();

        private int n;
        private int k;
        private BigInteger count;
        private int[] current;

        public int N
        {
            get { return n; }
        }
        public int K
        {
            get { return k; }
        }
        public BigInteger Count
        {
            get { return count; }
        }
        public int[] Current
        {
            get { return current; }
        }
        public int[] Next
        {
            get
            {
                var positions = new int[k];
                current.CopyTo(positions, 0);

                for (var i = k - 1; i >= 0; i--)
                {
                    positions[i]++;

                    if (i < k - 1)
                    {
                        for (var j = i + 1; j < k; j++)
                        {
                            positions[j] = positions[i] + j - i;
                        }
                    }

                    if (positions[i] > i + n - k)
                    {
                        if (i == 0)
                        {
                            for (var j = 0; j < k; j++)
                            {
                                positions[j] = j;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                current = positions;
                return current;
            }
        }
        public int[] Random
        {
            get
            {
                var positions = new int[k];

                for (var i = 0; i < k; i++)
                {
                    if (i == 0)
                        positions[i] = dice.Next(i + n - k + 1);
                    else
                        positions[i] = dice.Next(positions[i - 1] + 1, i + n - k + 1);
                }

                current = positions;
                return current;
            }
        }

        public CombinationEnumerator(int n, int k)
        {
            if (n <= 0)
                throw new ArgumentException(
                    $"{nameof(n)} can not be less than or equal to zero."
                );

            if (k <= 0)
                throw new ArgumentException(
                    $"{nameof(k)} can not be less than or equal to zero."
                );

            if (n < k)
                throw new ArgumentException(
                    $"{nameof(n)} can not be less than {nameof(k)}."
                );

            this.n = n;
            this.k = k;

            current = new int[k];
            for (var i = 0; i < k; i++)
            {
                current[i] = i;
            }

            Calculate();
        }

        private void Calculate()
        {
            if (k == 1)
                count = n;

            if (k == n)
                count = 1;

            if (k >= n / 2)
                count = ProductOfRange(n, k) / ProductOfRange(n - k, 1);
            else
                count = ProductOfRange(n, n - k) / ProductOfRange(k, 1);
        }

        private BigInteger ProductOfRange(int from, int to)
        {
            BigInteger product = 1;

            for (var i = from; i > to; i--)
            {
                product *= i;
            }

            return product;
        }
    }
}
