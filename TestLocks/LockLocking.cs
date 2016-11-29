using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestLocks
{
    class LockLocking
    {
        private int _numTasks;
        private object _object = new object();

        public LockLocking(int max = 10)
        {
            _numTasks = max;
        }


        public void Execute()
        {

            int x = 0;

            Action action = () =>
            {

                Stopwatch timeSpend = Stopwatch.StartNew();

                for (int i = 0; i < 100000; i++)
                {
                    lock(_object)
                    {

                        x += i;
                        Thread.SpinWait(50);
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
