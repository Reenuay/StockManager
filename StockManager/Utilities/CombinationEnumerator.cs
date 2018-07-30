using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace StockManager.Utilities {
    class CombinationEnumerator {
        private Random dice;
        private BigInteger count;
        private bool calculated;

        public int N { get; private set; }
        public int K { get; private set; }
        public int[] Current { get; private set; }

        public BigInteger Count {
            get {
                if (!calculated) {
                    Calculate();
                    calculated = true;
                }
                return count;
            }
        }

        public int[] Next {
            get {
                var positions = new int[K];
                Current.CopyTo(positions, 0);

                for (var i = K - 1; i >= 0; i--) {
                    positions[i]++;

                    if (i < K - 1) {
                        for (var j = i + 1; j < K; j++) {
                            positions[j] = positions[i] + j - i;
                        }
                    }

                    if (positions[i] > i + N - K) {
                        if (i == 0) {
                            for (var j = 0; j < K; j++) {
                                positions[j] = j;
                            }
                        }
                    }
                    else {
                        positions = Enumerable.Range(0, K).ToArray();
                        break;
                    }
                }

                Current = positions;
                return Current;
            }
        }

        public int[] Random {
            get {
                // Список всех элементов множества.
                var indexes = Enumerable.Range(0, N).ToArray();

                // Список приоритетов элементов множества в сортировке.
                var randomOrders = indexes.Select(RandomNumber).ToArray();

                // Отбираем k случайных элементов множества n.
                Current = indexes
                    .OrderBy(i => randomOrders[i])
                    .Take(K)
                    .OrderBy(i => i)
                    .ToArray();

                return Current;
            }
        }

        public CombinationEnumerator(int n = 0, int k = 0) {
            dice = new Random();
            ChangeValues(n, k);
        }

        public void ChangeValues(int n, int k) {
            calculated = false;
            count = 0;

            if (n < k)
                throw new ArgumentException(
                    $"{nameof(n)} can not be less than {nameof(k)}."
                );

            N = n;
            K = k;

            Current = new int[k];
            for (var i = 0; i < k; i++) {
                Current[i] = i;
            }
        }

        private void Calculate() {
            if (K == 0) {
                count = 0;
                return;
            }

            if (N == 0) {
                count = 0;
                return;
            }

            if (K == 1) {
                count = N;
                return;
            }

            if (K == N) {
                count = 1;
                return;
            }

            if (K >= N / 2)
                count = ProductOfRange(N, K) / ProductOfRange(N - K, 1);
            else
                count = ProductOfRange(N, N - K) / ProductOfRange(K, 1);
        }

        private BigInteger ProductOfRange(int from, int to) {
            BigInteger product = 1;

            for (var i = from; i > to; i--) {
                product *= i;
            }

            return product;
        }

        private int RandomNumber(int n) {
            return dice.Next();
        }
    }
}
