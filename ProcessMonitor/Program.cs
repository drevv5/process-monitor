using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace ProcessMonitor
{
    class Program
    {
        const int Minute = 60000; // Number of milliseconds in one minute.
        static DateTime StartTime; // Start time of this program.

        /// <summary>
        /// This method checks if user's arguments are correct to be used in program.
        /// </summary>
        /// <param name="args"></param>
        static void CheckArgs(string[] args)
        {
            try
            {
                if (args.Length != 3)
                    throw new ArgumentException("There should be 3 arguments: {processName} {killTime} {checkTime}");
                if (args.Any(value => String.IsNullOrEmpty(value)))
                    throw new ArgumentNullException("Some arguments are null or empty!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Process.GetCurrentProcess().Kill();
            }
        }

        /// <summary>
        /// This method parses user's arguments to a tuple consisting of string and two integers.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static (string, int, int) ParseArgs(string[] args)
        {
            
            int killTime = 0, checkTime = 0;
            bool parsed;
            try
            {
                parsed = Int32.TryParse(args[1], out killTime) && Int32.TryParse(args[2], out checkTime);
                if (!parsed)
                    throw new FormatException();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Process.GetCurrentProcess().Kill();
            }
            return (args[0], killTime, checkTime);
        }

        static void Main(string[] args)
        {
            string processName;
            int killTime, checkTime;

            Process[] processes;

            CheckArgs(args);
            (processName, killTime, checkTime) = ParseArgs(args);

            while (true)
            {
                StartTime = DateTime.Now;
                processes = Process.GetProcessesByName(processName);

                foreach (var proc in processes)
                {
                    var procTimeAlive = StartTime - proc.StartTime;

                    if (procTimeAlive.TotalMinutes > killTime)
                    {
                        proc.Kill();
                        Console.WriteLine($"Process \"{proc.ProcessName}\" (PID: {proc.Id}) has been killed.");
                    }
                }
                Thread.Sleep(Minute * checkTime);
            }
        }
    }
}
