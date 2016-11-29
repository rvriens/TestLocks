using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestLocks
{
    class BarrierLocking
    {

        private Barrier _barrier = null;
        private int _count = 0;

        public BarrierLocking(int max = 10)
        {

            _barrier = new Barrier(max, (b) =>
            {
                int ms = new Random().Next(0, 3);
                Display("Phase", b.CurrentPhaseNumber, ms, _count);


                if (b.CurrentPhaseNumber == 2)
                {
                    throw new Exception("Woops in phase 2");
                }
            });

        }

        public void Execute()
        {

            //Display("Add participants", 0, 0, 0);
            //_barrier.AddParticipants(2);

            //Display("Rem participants", 0, 0, 0);
            //_barrier.RemoveParticipant();

            //_barrier.RemoveParticipant(1);


            Action action = () =>
            {

                Interlocked.Increment(ref _count);
                _barrier.SignalAndWait();

                Interlocked.Increment(ref _count);
                _barrier.SignalAndWait();

                Interlocked.Increment(ref _count);
                try
                {
                    _barrier.SignalAndWait();
                }
                catch (BarrierPostPhaseException bppe)
                {
                    Display("Ex" + bppe.Message, 0, 0, 0);
                }




                // The fourth time should be hunky-dory
                Interlocked.Increment(ref _count);
                _barrier.SignalAndWait(); // during the post-phase action, count should be 16 and phase should be 3
            };

            IList<Action> list = new List<Action>();

            for (int i = 0; i < _barrier.ParticipantCount; i++)
            {
                list.Add(action);
            }

            Parallel.Invoke(list.ToArray());

            //List<Task> tasks = new List<Task>();
            //for (int i = 0; i < _barrier.ParticipantCount; i++)
            //{
            //    var t = new Task(action);
            //    t.Start();
            //    tasks.Add(t);
            //}

            //Task.WaitAll(tasks.ToArray());

        }

        private void Display(string t, long x, int ms, int c)
        {
            Console.WriteLine(string.Format("{0,-20}, {1,5} - {2,5}ms - {3,5}", t, x, ms, c));
        }

    }
}
