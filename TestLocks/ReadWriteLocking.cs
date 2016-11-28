using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestLocks
{
    class ReadWriteLocking
    {
        private ReaderWriterLock _readerWriterLock = null;
        private List<Task> _tasks = null;
        private int _counter = 0;

        public ReadWriteLocking(int max = 10)
        {
            _readerWriterLock = new ReaderWriterLock();
            _tasks = new List<Task>();
        }


        public void Execute()
        {
            Console.WriteLine("Execute");
            for (int i = 0; i < 1000; i++)
            {
                _tasks.Add(ExecuteTask(i));
            }
            //_tasks.ForEach(async q => await q);
            Task.WaitAll(_tasks.ToArray());
        }

        private async Task ExecuteTask(int x)
        {

            try
            {
                //_readerWriterLock.AcquireReaderLock(20);
                //try
                //{
                    Task task = DoReadAsync(x);
                    await DoWriteAsync(x);
                    await task;

                //}
                //finally
                //{
                //    _readerWriterLock.ReleaseReaderLock();
                //}
            }
            catch (ApplicationException ex)
            {
                Console.WriteLine(string.Format("Timout execute {0}", x));
            }

        }

        private Task DoWriteAsync(int x)
        {
            var t = new Task(() =>
            {
                DoWrite(x);
            });
            t.Start();
            return t;
        }

        private void DoWrite(int x)
        {
            try
            {
                _readerWriterLock.AcquireReaderLock(40);
                try
                {
                    try
                    {
                        int ms = new Random().Next(0, 3);
                        int c = 0;
                        LockCookie lc = _readerWriterLock.UpgradeToWriterLock(60);
                        try
                        {
                            c = _counter;

                            // thread switch
                            Thread.Sleep(ms);

                            _counter = c + 1;

                        }
                        finally
                        {
                            // Ensure that the lock is released.
                            _readerWriterLock.DowngradeFromWriterLock(ref lc);

                            Display("Write Done", x, ms, c);
                        }
                    }
                    catch (ApplicationException ex)
                    {
                        Display("Timout upgrade", x, 0, 0);
                        Thread.Sleep(1);
                        DoWrite(x);
                    }
                }
                finally
                {
                    _readerWriterLock.ReleaseReaderLock();
                }
            }
            catch
            {
                Display("Timeout Writer", x, 0, 0);
                DoWrite(x);
            }
        }

        private void Display(string t, int x, int ms, int c)
        {
            Console.WriteLine(string.Format("{0,-20}, {1,5} - {2,5}ms - {3,5}", t, x, ms, c));
        }


        private Task DoReadAsync(int x)
        {
            var t = new Task(() =>
            {
                DoRead(x);
            });
            t.Start();
            return t;

        }

        private void DoRead(int x)
        {

            int lastWriter;

            try
            {
                _readerWriterLock.AcquireReaderLock(30);
                try
                {
                    int resourceValue = _counter;
                    lastWriter = _readerWriterLock.WriterSeqNum;
                    LockCookie lc = _readerWriterLock.ReleaseLock();
                    int ms = new Random().Next(0, 3);

                    Thread.Sleep(ms);
                    _readerWriterLock.RestoreLock(ref lc);

                    if (_readerWriterLock.AnyWritersSince(lastWriter))
                    {
                        resourceValue = _counter;
                        Display("Cache Changed", x, ms, resourceValue);
                    }
                    else
                    {
                        Display("Cache Cached", x, ms, resourceValue);
                    }

                }
                finally
                {
                    _readerWriterLock.ReleaseReaderLock();
                }

            }
            catch
            {
                Display("Timeout Reader", x, 0, 0);
            }
        }
    }
}
