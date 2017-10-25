using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Threading;
using System.IO.Ports;

namespace ClassLibrary1
{
    public class PiReader
    {
        SerialPort com;
        byte[] b = new byte[8];
        Thread thread;
        char[] char_array;

        public struct data
        {
        }

        public data GetData()
        {
            data stuff = new data()
            {
            };
            return stuff;
        }

        
        public PiReader()
        {
            Console.WriteLine("setting up serial reader");

            com = new SerialPort();
            com.PortName = "COM7";
            com.BaudRate = 115200;
            com.DataBits = 8;
            com.Parity = Parity.None;
            com.StopBits = StopBits.One;

            // virtual config
            // com.PortName = "COM4";
            // com.BaudRate = 115200;
            // com.DataBits = 8;
            // com.Parity = Parity.None;
            // com.StopBits = StopBits.One;
            
            Console.WriteLine(com);
            com.Open();

            byte[] a = Enumerable.Range(0, 16).Select(i => (byte) i).ToArray();
            Console.WriteLine("reading bytes");
            com.Write(a, 0, 16);
            byte[] by = new byte[16];
            com.Read(by, 0, 16);

            
            BackgroundWorker thread = new BackgroundWorker();
            thread.DoWork += Data_Getter;
            thread.RunWorkerAsync();
            
        }

        private void Data_Getter(object sender, DoWorkEventArgs e)
        {
            List<byte> bytes = new List<byte>();
            while(true)
            {
                // bytes read in are added to an array list
                // they are popped off that array list once a command is recognized
                com.Read(b, 0, 1);
                if (b[0] == 0x00)
                {
                    // end of command
                    ExecuteCommand(bytes);
                    bytes.Clear();
                    continue;
                }
                else
                {
                    bytes.Add(b[0]);
                }
            }
        }
    private void ExecuteCommand(List<byte> bytes)
        {
            bytes = Consistent_Overhead_Byte_Stuffing.COBS.Decode(bytes).ToList<byte>();
            foreach(byte by in bytes)
            {
                Console.Write(by);
            }
            Console.WriteLine();
        }
    }
}

