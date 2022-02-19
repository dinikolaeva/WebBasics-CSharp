using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ParallelForAndForeach
{
    class Program
    {
        static int Count = 0;
        static object lockObj = new object();

        static void Main(string[] args)
        {
            Stopwatch sw = Stopwatch.StartNew();
            PrintPrimeCount(1, 10_000);
            Console.WriteLine(sw.Elapsed);
        }

        static void PrintPrimeCount(int min, int max)
        {
            //synchronous:
            //for (int i = min; i <= max; i++)

            //parallel For:
            Parallel.For(min, max + 1, i =>
            {
                bool isPrime = true;

                for (int j = 2; j <= Math.Sqrt(i); j++)
                {
                    if (i % j == 0)
                    {
                        isPrime = false;
                        break;
                    }

                    if (isPrime)
                    {
                        lock (lockObj)
                        {
                            Count++;
                        }
                    }
                }

            });
            Console.WriteLine(Count);
        }
    }
}
