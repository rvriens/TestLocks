using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestLocks
{
    class MutexLocking
    {

        private Mutex _m;
        private int _numTasks = 0;
        const string MUTEX_NAME = "TestingMutext";

        public MutexLocking(int max = 10)
        {
            _numTasks = max;
        }

        public void Execute()
        {

            ProcessStartInfo procStartInfo = new ProcessStartInfo();
            procStartInfo.UseShellExecute = true;
            procStartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            procStartInfo.FileName = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;  //Application.ExecutablePath;
            procStartInfo.Arguments = "mutexlocking";

            ManualResetEvent finishedEvent = new ManualResetEvent(false);
            CountdownEvent endCountdown = new CountdownEvent(_numTasks);

            Action action = () =>
            {
                Stopwatch swProcess = Stopwatch.StartNew();

                Process process = null;
                try
                {
                    process = new Process();
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.ErrorDialog = false;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    process.StartInfo.FileName = System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", ""); ;  //Application.ExecutablePath;
                    process.StartInfo.Arguments = "mutexlocking";

                    process.ErrorDataReceived += (sendingProcess, errorLine) =>
                    {
                        if (errorLine.Data == null)
                        {
                            return;
                        }
                        Display("err: " + errorLine.Data);
                    };
                    process.OutputDataReceived += (sendingProcess, dataLine) =>

                    {
                        if (dataLine.Data == null)
                        {
                            return;
                        }

                        if (dataLine.Data.Contains("instance"))
                        {
                            swProcess.Stop();
                            Console.WriteLine(string.Format("Start process spend {0}ms", swProcess.ElapsedMilliseconds));

                            endCountdown.Signal();
                        }

                        Display(dataLine.Data);
                    };

                    process.Start();
                    process.BeginErrorReadLine();
                    process.BeginOutputReadLine();

                    finishedEvent.WaitOne();
                    process.StandardInput.WriteLine("");
                    process.WaitForExit();

                }
                catch (Exception ex)
                {
                    Display("err: " + ex.Message);
                }
            };

            IList<Task> tasks = new List<Task>();

            for (int i = 0; i < _numTasks; i++)
            {
                var t = new Task(action);
                t.Start();
                tasks.Add(t);
            }

            endCountdown.Wait();

            Display("All process finished");

            finishedEvent.Set();
            Task.WaitAll(tasks.ToArray());

        }

        public void DoMutexLocking()
        {
            Stopwatch sw = Stopwatch.StartNew();

            if (!IsSingleInstance())
            {
                Console.WriteLine("More than one instance"); // Exit program.
            }
            else
            {
                Console.WriteLine("One instance <======================= This one"); // Continue with program.
            }

            sw.Stop();
            Console.WriteLine(string.Format("Spend {0}ms", sw.ElapsedMilliseconds));

            Console.ReadLine();
            if (_m != null)
            {
                _m.Close();
            }
        }

        bool IsSingleInstance()
        {
            bool createdNew = false;

            try
            {
                _m = Mutex.OpenExisting(MUTEX_NAME);
            }
            catch
            {
                _m = new Mutex(true, MUTEX_NAME, out createdNew);
            }
            return createdNew;
        }

        private void Display(string t)
        {
            Console.WriteLine(string.Format("{0}", t));
        }


    }
}
