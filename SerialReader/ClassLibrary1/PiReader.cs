﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Threading;
using System.IO.Ports;


namespace PiTracker
{
    public class PiReader
    {
        ISerial pi;
        byte[] b = new byte[8];
        Thread thread;
        char[] char_array;
        HeadTracker ht;

        enum Commands { CameraDistortionCalibration = 0x01,
                        ResetPositional = 0x02,
                        SetEyeDistance = 0x03,
                        AddCalibrationPoint = 0x04,
                        OutputConsole = 0x05,
                        PositionData = 0x06,
                      }

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

        
        public PiReader(HeadTracker ht, ISerial piSerial)
        {
            pi = piSerial;

            byte[] a = Enumerable.Range(0, 16).Select(i => (byte) i).ToArray();
            Console.WriteLine("reading bytes");
            pi.Write(a, 0, 16);
            byte[] by = new byte[16];
            pi.Read(by, 0, 16);

            this.ht = ht;
            
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
                pi.Read(b, 0, 1);
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
            Commands type = (Commands)bytes[0];
            switch (type)
            {
                case Commands.CameraDistortionCalibration:
                    int camera = (int)bytes[1];
                    ht.StartDistortionCalibration(camera);
                    break;
                case Commands.ResetPositional:
                    ht.ResetPositionalCalibration();
                    break;
                case Commands.SetEyeDistance:
                    System.Single distance = System.BitConverter(bytes, 1);
                    break;
                case Commands.OutputConsole:
                    break;
                case Commands.PositionData:
                    break;
            }
        }
    }
}

