using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestLocks
{
    class ManualResetEventLocking
    {
        private int _threadsCount = 0;

        public ManualResetEventLocking(int max = 10)
        {
            _threadsCount = max;
        }

        public void Execute()
        {
            int x = 0;

            ManualResetEvent _startEvent = new ManualResetEvent(false);
            CountdownEvent _endCountdown = new CountdownEvent(_threadsCount);

            Thread[] threads = new Thread[_threadsCount];

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(() =>
                {
                    _startEvent.WaitOne();

                    for (int j = 0; j < 10000; j++)
                    {
                        Interlocked.Increment(ref x);
                    }

                    _endCountdown.Signal();
                });

                threads[i].Priority = ThreadPriority.Normal;
                threads[i].Start();
            }

            Stopwatch sw = Stopwatch.StartNew();

            _startEvent.Set();
            _endCountdown.Wait();

            sw.Stop();

            Display("Result: ", x);

            Display("Ms spend", sw.ElapsedMilliseconds);
        }

        private void Display(string t, long x)
        {
            Console.WriteLine(string.Format("{0,-20} {1}", t, x));
        }

    }
}
