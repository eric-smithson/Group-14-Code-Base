using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("beginning program");
            PiTracker.PiSerial ps = new PiTracker.PiSerial();
            PiTracker.PiReader m = new PiTracker.PiReader(new PiTracker.HeadTracker(ps), ps);

            Console.WriteLine("Press Enter to Exit the Program");
            Console.Read();
        }
    }
}
