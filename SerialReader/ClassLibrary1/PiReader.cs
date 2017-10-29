using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Threading;
using System.IO.Ports;
using System.Diagnostics;

namespace PiTracker
{
    // PiReader Class has methods to read from the Pi
    public class PiReader
    {
        ISerial pi;
        byte[] b = new byte[8];
        HeadTracker ht;

        public enum Commands {
                                CameraDistortionCalibration = 0x01,
                                ResetPositional = 0x02,
                                SetEyeDistance = 0x03,
                                AddCalibrationPoint = 0x04,
                                OutputConsole = 0x05,
                                UpdatePositionData = 0x06,
                             }

        public PiReader(HeadTracker ht, ISerial piSerial)
        {
            pi = piSerial;

            // pi.Read(by, 0, 16);


            /*pi.Write(a, 0, 16);
            byte[] by = new byte[16];
            pi.Read(by, 0, 16);*/

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
                int read = pi.Read(b, 0, 1);
                Console.WriteLine(b[0]);
                if (read == 1)
                {
                    if (b[0] == 0x00)
                    {
                        // end of command
                        ReadCommand(bytes);
                        bytes.Clear();
                        continue;
                    }
                    else
                    {
                        bytes.Add(b[0]);
                    }
                }
                Thread.Sleep(0);
            }
        }
        private void ReadCommand(List<byte> bytes)
        {
            if (bytes.Count <= 0) {
                return;
            }
            List<byte> b_arr;
            b_arr = Consistent_Overhead_Byte_Stuffing.COBS.Decode(bytes).ToList<byte>();
            foreach(byte by in b_arr)
            {
                Debug.Write(by);
            }
            if (b_arr.Count <= 0) {
                Debug.Write("command length 0");
                return;
            }
            Debug.Write("\n");
            Commands type = (Commands)b_arr[0];
            b_arr.RemoveAt(0);
            switch (type)
            {
                case Commands.OutputConsole:
                    ht.ReceiveOutput(b_arr);
                    break;
                case Commands.UpdatePositionData:
                    ht.UpdatePosition(b_arr);
                    break;
                default:
                    Debug.WriteLine("Command not found");
                    break;
            }
        }
        public void WriteCommand(Commands type, List<byte> data)
        {
            data.Insert(0, (byte)type);
            data = Consistent_Overhead_Byte_Stuffing.COBS.Decode(data).ToList<byte>();
            data.Add(0x00);
            pi.Write(data.ToArray(), 0, data.Count);
        }
    }
}

