using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestLocks
{
    class LazyInitializerLocking
    {

        private int _numThreads = 0;
        private Customer customer = null;

        public LazyInitializerLocking(int max = 10)
        {
            _numThreads = max;
        }

        public void Execute()
        {
            customer = new Customer();
            Thread[] threads = new Thread[_numThreads];
            for (int i = 0; i < _numThreads; i++)
            {
                Thread newThread = new Thread(new ThreadStart(
                    () =>
                    {
                        this.DoLazy2();
                        this.DoLazy();
                    }
                    
                    ));
                newThread.Start();
                threads[i] = newThread;
            }
            threads.ToList().ForEach((t) => t.Join());
        }
        
        public void DoLazy()
        {
            Stopwatch sw = Stopwatch.StartNew();
            customer.GetPayment(new Random().Next(1, 4));
            sw.Stop();
            Display("Time spend: ", sw.ElapsedMilliseconds);
        }

        public void DoLazy2()
        {
            Stopwatch sw = Stopwatch.StartNew();
            
            customer.GetPayment2(new Random().Next(1, 4));
            sw.Stop();
            Display("Time spend (2): ", sw.ElapsedMilliseconds);
        }



        private void Display(string t, long x)
        {
            Console.WriteLine(string.Format("{0,-20} {1}", t, x));
        }



        public class Customer
        {
            private IList<Payment> _payments = null;
            private bool _paymentsInitialized = false;
            private Lazy<IList<Payment>> _payments2 = null;
            private object _syncLock = new object();

            public Customer()
            {
                _payments2 = new Lazy<IList<Payment>>(() => this.FetchPayments(), LazyThreadSafetyMode.ExecutionAndPublication);
            }

            public string Name { get; set; }
            public IList<Payment> Payments
            {
                get
                {
                    return LazyInitializer.EnsureInitialized<IList<Payment>>(ref _payments, ref _paymentsInitialized, ref _syncLock, () => this.FetchPayments());
                }
            }

            public Lazy<IList<Payment>> Payments2
            {
                get
                {
                    return _payments2;
                }
            }

            private IList<Payment> FetchPayments()
            {
                List<Payment> payments = new List<Payment>();
                payments.Add(new Payment { BillNumber = 1, BillDate = DateTime.Now, PaymentAmount = 200 });
                payments.Add(new Payment { BillNumber = 2, BillDate = DateTime.Now.AddDays(-1), PaymentAmount = 540 });
                payments.Add(new Payment { BillNumber = 3, BillDate = DateTime.Now.AddDays(-2), PaymentAmount = 700 });
                payments.Add(new Payment { BillNumber = 4, BillDate = DateTime.Now, PaymentAmount = 500 });
                //Load all the payments here from database
                return payments;
            }

            public Payment GetPayment(int billno)
            {

                    Payment p = this.Payments.FirstOrDefault(pay => pay.BillNumber.Equals(billno));
                    return p;
            }

            public Payment GetPayment2(int billno)
            {
                var x = this.Payments2.Value;
                if (this.Payments2.IsValueCreated)
                {
                    var payments = this.Payments2.Value;
                    Payment p = payments.FirstOrDefault(pay => pay.BillNumber.Equals(billno));
                    return p;
                }
                return null;
                //else
                //    throw new NotImplementedException("Object is not initialized");
            }

        }

        public class Payment
        {
            public int BillNumber { get; set; }
            public DateTime BillDate { get; set; }
            public double PaymentAmount { get; set; }
        }
    }
}