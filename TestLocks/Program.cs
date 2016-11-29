using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLocks
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Contains("mutexlocking")) {

                new MutexLocking().DoMutexLocking();
             
                return;
            }

            Console.WriteLine("Testing Locking Methods");

            new BarrierLocking(7).Execute();
            new InterLocking(3).Execute();
            new LazyInitializerLocking(10).Execute();
            new LockLocking(3).Execute();
            new ManualResetEventLocking(8).Execute();
            new MonitorLocking(3).Execute();
            new MutexLocking(4).Execute();
            new ReadWriteLocking(10).Execute();
            new SemaphoreLocking(50).Execute();
            new SpinLocking(3).Execute();
            new ThreadLocalLocking(10).Execute();
           

            Console.WriteLine("Press <ENTER> to exit...");
            Console.ReadLine();

        }
    }
}
