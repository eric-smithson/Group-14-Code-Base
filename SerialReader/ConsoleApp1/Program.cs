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

            Console.WriteLine("Sending Data");
            ps.Write(a, 0, 16);
            Console.WriteLine("Bytes sent, waiting for bytes to be returned");
            for(int i = 0; i < 16; i++)
            {
                ps.Read(buffer, 0, 1);
                Console.WriteLine(buffer[0]);
            }
            Console.WriteLine("Bytes read");
            
            for(int i = 0; i < 16; i++)
            {
                Console.Write(buffer[i]);
            }
            Console.WriteLine();

        }

        private static void Thread_DoWork(object sender, DoWorkEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
