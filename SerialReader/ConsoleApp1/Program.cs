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


            byte[] buffer = new byte[16];
            byte[] a = Enumerable.Range(0, 16).Select(i => (byte) i).ToArray();
            ps.Write(a, 0, 16);
            ps.Read(buffer, 0, 16);
            
            for(int i = 0; i < 16; i++)
            {
                Console.Write(buffer[i]);
            }
            Console.WriteLine();

            while (true)
            {
                Console.WriteLine("Press Enter to read bits:");
            }

        }

        private static void Thread_DoWork(object sender, DoWorkEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
