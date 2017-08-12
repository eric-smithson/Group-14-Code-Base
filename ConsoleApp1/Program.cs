using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            ClassLibrary1.MyUtilities m = new ClassLibrary1.MyUtilities();
            m.SetData();
            ClassLibrary1.MyUtilities.data r = m.GetData();
            
            Console.WriteLine(r.accelx);
            Console.WriteLine(r.accely);
            Console.WriteLine(r.accelz);

            Console.WriteLine(r.gyrox );
            Console.WriteLine(r.gyroy );
            Console.WriteLine(r.gyroz );
        }
    }
}
