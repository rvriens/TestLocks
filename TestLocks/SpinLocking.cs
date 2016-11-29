using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestLocks
{
    class SpinLocking
    {
        private int _numTasks;

        public SpinLocking(int max = 10)
        {
            _numTasks = max;
        }

        public void Execute()
        {
            SpinLock sl = new SpinLock();

            int x = 0;

            Action action = () =>
            {

                Stopwatch timeSpend = Stopwatch.StartNew();

                bool gotLock = false;
                for (int i = 0; i < 100000; i++)
                {
                    gotLock = false;
                    try
                    {
                        sl.Enter(ref gotLock);
                        x += i;
                        Thread.SpinWait(50);
                    }
                    finally
                    {
                    if (gotLock) sl.Exit();
                    }
                    Thread.SpinWait(50);
                }
                timeSpend.Stop();
                Display("Ms spend", timeSpend.ElapsedMilliseconds);
                
            };

            IList<Action> listActions = new List<Action>();

            for (int i = 0; i < _numTasks; i++)
            {
                listActions.Add(action);
            }

            Parallel.Invoke(listActions.ToArray());

            Display("Result: ", x);
        }

        private void Display(string t, long x)
        {
            Console.WriteLine(string.Format("{0,-20} {1}", t, x));
        }

    }
}
