using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;

namespace ProcessMonitoring
{
    public class ProcessMonitor
    {
        public string ProcessName { get; private set; } // Target process name.
        public int KillTime { get; private set; } = 5;  // Wait this ammount of minutes to kill an application. Default: 5
        public int CheckTime { get; private set; } = 1; // Wait this ammount of minutes to check if application is still alive. Default: 1
        private DateTime LoopStartTime { get; set; } // Start time of process monitor.

        public ProcessMonitor() { }

        public ProcessMonitor(string processName) => ProcessName = processName;

        public ProcessMonitor(Process process) => this.ProcessName = process.ProcessName;

        public ProcessMonitor(string processName, int kill)
        {
            ProcessName = processName;
            KillTime = kill;
        }

        public ProcessMonitor(Process process, int kill)
        {
            ProcessName = process.ProcessName;
            KillTime = kill;
        }

        public ProcessMonitor(string processName, int kill, int check)
        {
            ProcessName = processName;
            KillTime = kill;
            CheckTime = check;
        }

        public ProcessMonitor(Process process, int kill, int check)
        {
            ProcessName = process.ProcessName;
            KillTime = kill;
            CheckTime = check;
        }

        public ProcessMonitor(params string[] args)
        {
            if (!ArgsAreCorrect(args))
                return;
            (ProcessName, KillTime, CheckTime) = ParseArgs(args);
        }

        private bool IsDigitsOnly(string s) =>
            s.All(c => c >= '0' && c <= '9');

        private bool IsDigitsOnly(params string[] s) =>
            s.All(c => IsDigitsOnly(c));

        /// <summary>
        /// This method checks if user's arguments are correct to be used in program.
        /// </summary>
        /// <param name="args"></param>
        public bool ArgsAreCorrect(string[] args)
        {
            if (args.Any(value => string.IsNullOrEmpty(value)))
                throw new ArgumentNullException();
            if (args.Length != 3)
                throw new WrongAmmountOfArgumentsException();
            if (Process.GetProcessesByName(args[0]).Length == 0)
                throw new ArgumentException("No such processes found!");
            if (!IsDigitsOnly(args[1], args[2]))
                throw new ArgumentException("Not correct ammount of time!");
            if (int.Parse(args[1]) < int.Parse(args[2]))
                throw new ArithmeticException("");
            return true;
        }

        /// <summary>
        /// This method parses user's arguments to a tuple consisting of string and two integers.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public (string, int, int) ParseArgs(string[] args)
        {
            int killTime = this.KillTime;
            int checkTime = this.CheckTime;
            if (!(int.TryParse(args[1], out killTime) || int.TryParse(args[2], out checkTime)))
                throw new FormatException();
            return (args[0], killTime, checkTime);
        }

        private void CheckAndKillLoop(Process[] processes)
        {
            foreach (var (proc, procTimeAlive) in
                    from proc in processes
                    let procTimeAlive = LoopStartTime - proc.StartTime
                    select (proc, procTimeAlive))
            {
                Console.WriteLine("Process \"{0}\" (PID: {1}) is now alive for {2}.",
                    proc.ProcessName, proc.Id, procTimeAlive);
                if (procTimeAlive.TotalMinutes > KillTime)
                    proc.Kill();
                Console.WriteLine("Process \"{0}\" (PID: {1}) has been killed.",
                    proc.ProcessName, proc.Id);
            }
        }

        public void Start()
        {
            Process[] processes = Process.GetProcessesByName(ProcessName);

            if (processes.Length > 0)
                CheckAndKillLoop(processes);
            else
                Console.WriteLine("No such processes alive! (\"{0}\")", ProcessName);
            return;
        }

        public void Loop()
        {
            Process[] processes;
            TimeSpan checkTimeSpan = TimeSpan.FromMinutes(1d) * CheckTime;

            while (true)
            {
                LoopStartTime = DateTime.Now;
                processes = Process.GetProcessesByName(ProcessName);
                if (processes.Length == 0)
                {
                    Console.WriteLine("No such processes alive! (\"{0}\")", ProcessName);
                    break;
                }
                else
                {
                    CheckAndKillLoop(processes);
                    Console.WriteLine("Waiting for next check...");
                    Thread.Sleep(checkTimeSpan);
                    Console.WriteLine("Checking...");
                }
            }
        }
    }
}
