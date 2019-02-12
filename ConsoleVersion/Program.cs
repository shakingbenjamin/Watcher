using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WatcherLibrary;

namespace ConsoleVersion
{
    class Program
    {
        //private static Timer ticker;
        static void Main(string[] args)
        {
            // console test out put
            var ticker = new Timer(10000);
            var aw = new ActiveWindow();
            ticker.Elapsed += new ElapsedEventHandler(aw.OnTimeAppend);
            Microsoft.Win32.SystemEvents.SessionSwitch += new Microsoft.Win32.SessionSwitchEventHandler(aw.SystemEvents_SessionSwitchAppend);
            ticker.Enabled = true;
            ticker.Interval = 2000;
            Console.WriteLine("Press something to exit");
            Console.ReadKey();

            // calls the library
            //var start = new StartUp();
            //start.GoTimeAppend();
        }
    }
}
