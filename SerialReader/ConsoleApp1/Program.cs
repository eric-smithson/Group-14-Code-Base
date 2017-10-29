using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("beginning program");

            byte[] a = Enumerable.Range(1, 17).Select(i => (byte) i).ToArray();
            Debug.WriteLine("writing bytes");
            PiTracker.HeadTracker.Singleton.serial.Write(a, 0, 16);
            byte[] by = new byte[16];

            // Debug.WriteLine("reading bytes");
            Console.WriteLine("Press Enter to Exit the Program");
            Console.Read();
        }
    }
}
