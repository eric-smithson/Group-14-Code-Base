using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.IO.TextWriter;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Ports;
using UnityEngine;

namespace ClassLibrary1
{
    public class MyUtilities
    {
        SerialPort com;
        byte[] buffer;
        Thread thread;

        public struct data
        {
            public int gyrox;
            public int gyroz;
            public int gyroy;

            public int accelx;
            public int accely;
            public int accelz;
        }

        public data GetData()
        {
            data stuff = new data()
            {
                // This is the same order I expect to receive the bytes in
                gyrox  = buffer[0],
                gyroy  = buffer[1],
                gyroz  = buffer[2],

                accelx = buffer[3],
                accely = buffer[4],
                accelz = buffer[5],
            };
            return stuff;
        }

        // design choice: do we read as fast as we can here?
        // design answer: yes, don't worry about the system looping
        //       Because the system will just sleep while waiting for new data.
        //       Data will be bottlenecked on the Pi side of things
        
        public void SetData()
        {
            while(true)
            {
                byte[] temp = new byte[6];
                com.Read(temp, 0, 6);
                for (int i = 0; i < 6; i++) {
                    buffer[i] = temp[i];
                } 
            }

        }

        public MyUtilities()
        {
            com = new SerialPort();
            com.PortName = "COM4";
            com.BaudRate = 19200;
            com.DataBits = 8;
            com.Parity = Parity.None;

            com.Open();

            buffer = new byte[6];
            
            // thread = new Thread(new ThreadStart(SetData));
            // thread.Start();
        }
    }
}

