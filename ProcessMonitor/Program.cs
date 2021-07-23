using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessMonitoring
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var pm = new ProcessMonitor(args);
            pm.Loop();
            return 0;
        }
    }
}
