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
            ClassLibrary1.PiReader m = new ClassLibrary1.PiReader();
            
            while (true)
            {
                Console.WriteLine("Press Enter to read bits:");
                ClassLibrary1.PiReader.data r = m.GetData();
                Console.ReadLine();
                Console.WriteLine("{0}", r.gyrox);
                Console.WriteLine("{0}", r.gyroy);
                Console.WriteLine("{0}", r.gyroz);
                
                Console.WriteLine("{0}", r.accelx);
                Console.WriteLine("{0}", r.accely);
                Console.WriteLine("{0}", r.accelz);
            }

        }

        private static void Thread_DoWork(object sender, DoWorkEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
