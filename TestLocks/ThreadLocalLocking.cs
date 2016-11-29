using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestLocks
{
    class ThreadLocalLocking
    {

        private int _numThreads = 0;
        public ThreadLocalLocking(int max = 10)
        {
            _numThreads = max;
        }

        public void Execute()
        {

            Thread[] threads = new Thread[_numThreads];
            for (int i = 0; i < _numThreads; i++)
            {
                Thread newThread = new Thread(new ThreadStart(this.CreateThreadLocal));
                newThread.Start();
                threads[i] = newThread;
            }
        

            threads.ToList().ForEach((t) => t.Join());


        }

        

        public void CreateThreadLocal()
        {
            ThreadLocal<List<float>> local = new ThreadLocal<List<float>>(() => this.GetNumberList(Thread.CurrentThread.ManagedThreadId));

            //Thread.Sleep(50);

            List<float> numbers = local.Value;
            foreach (float num in numbers)
                Console.WriteLine(num);

        }

        private List<float> GetNumberList(int p)
        {
            Random rand = new Random(p);
            List<float> items = new List<float>();
            for (int i = 0; i < 10; i++)
                items.Add(rand.Next());
            return items;
        }

        
    }
}
