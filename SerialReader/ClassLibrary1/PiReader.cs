using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.IO.TextWriter;
using System.ComponentModel;
using System.Threading;
using System.IO.Ports;
using UnityEngine;
using System.Collections;

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
            public byte gyrox;
            public byte gyroz;
            public byte gyroy;

            public byte accelx;
            public byte accely;
            public byte accelz;
        }

        public data GetData()
        {
            data stuff = new data()
            {
                // This is the same order I expect to receive the bytes in
                gyrox  = b[0],
                gyroy  = b[1],
                gyroz  = b[2],

                accelx = b[3],
                accely = b[4],
                accelz = b[5],
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
            bool command = false;
            while(true)
            {
                // bytes read in are added to an array list
                // they are popped off that array list once a command is recognized
                com.Read(b, 0, 1);
                if (b[0] == 0x00 && command == false)
                {
                    // beginning of command
                    command = true;
                    continue;
                }
                if (b[0] == 0x00 && command == true)
                {
                    // end of command
                    command = false;
                    ExecuteCommand(bytes);
                    bytes.Clear();
                    continue;
                }
                if (command == true)
                {
                    bytes.Add(b[0]);
                }
            }
        }
    private void ExecuteCommand(List<byte> bytes)
        {
            foreach(byte by in bytes)
            {
                Console.Write(by);
            }
            Console.WriteLine();
        }
    }
}

