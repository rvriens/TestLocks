using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestLocks
{
    class SemaphoreLocking
    {
        private Semaphore _semaphore = null;
        private List<Task> _tasks = null;

        private int _doneCounter;
        public int DoneCounter { get { return _doneCounter; } }
        public void IncrementDoneCounter() { Interlocked.Increment(ref _doneCounter); }
        public void DecrementDoneCounter() { Interlocked.Decrement(ref _doneCounter); }

        public SemaphoreLocking(int max = 10)
        {
            _semaphore = new Semaphore(max, max);
            _tasks = new List<Task>();
        }

        public void Execute()
        {
            Console.WriteLine("Execute");
            for (int i = 0; i < 1000; i++)
            {
                _semaphore.WaitOne();
                _tasks.Add(ExecuteTask(i));
            }
            //_tasks.ForEach(async q => await q);
            Task.WaitAll(_tasks.ToArray());
        }

        private async Task ExecuteTask(int x)
        {
            IncrementDoneCounter();

            Task task = DoTaskAsync(x);

            await Do2();
            await task;

            DecrementDoneCounter();
            _semaphore.Release();
            
        }

        private Task Do2()
        {
            var t = new Task(() =>
            {
                Thread.Sleep(2);
            });
            t.Start();
            return t;

        }

        private Task DoTaskAsync(int x)
        {
            var t = new Task( () =>
                {
                    int ms = new Random().Next(0, 20);
                    Thread.Sleep(ms);
                                      
                    Console.WriteLine(string.Format("Test {0} - {1}ms - {2}", x, ms, DoneCounter));
                });
            t.Start();
            return t;
            
        }

    }
}
