using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace EmotionalEstimation
{
    class PraatManager
    {
        public static void Start()
        {
            ProcessStartInfo s_info = new ProcessStartInfo();
            s_info.FileName = Environment.CurrentDirectory + "/praat/praat.exe";
            s_info.WindowStyle = ProcessWindowStyle.Minimized;
            s_info.WorkingDirectory = Environment.CurrentDirectory;
            s_info.CreateNoWindow = true;
            Process.Start(s_info);
        }
        public static void Stop()
        {
            var prcs=Process.GetProcessesByName("praat");
            foreach (var prc in prcs)
            {
                prc.Kill();
            }
        }
        public static bool IsWorking()
        {
            if (Process.GetProcessesByName("praat").Length != 0)
                return true;
            else
                return false;
                
        }
        public static void Execute(string command)
        {
            ProcessStartInfo s_info = new ProcessStartInfo();
            s_info.FileName = Environment.CurrentDirectory + "/praat/sendpraat.exe";
            s_info.WindowStyle = ProcessWindowStyle.Minimized;
            s_info.WorkingDirectory = Environment.CurrentDirectory;
            s_info.CreateNoWindow = true;
            s_info.Arguments = "praat "+command;

            var process=Process.Start(s_info);
            while (!process.HasExited) ;
        }
    }
}
