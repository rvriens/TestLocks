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
            Console.WriteLine("Testing Semaphore");

            new SemaphoreLocking(50).Execute();


            Console.WriteLine("Press <ENTER> to exit...");
            Console.ReadLine();

        }
    }
}
