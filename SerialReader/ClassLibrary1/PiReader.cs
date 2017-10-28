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

            byte[] a = Enumerable.Range(0, 16).Select(i => (byte) i).ToArray();
            Debug.WriteLine("writing bytes");
            pi.Write(a, 0, 16);
            byte[] by = new byte[16];
            Debug.WriteLine("reading bytes");
            pi.Read(by, 0, 16);

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
            bytes = Consistent_Overhead_Byte_Stuffing.COBS.Decode(bytes).ToList<byte>();
            foreach(byte by in bytes)
            {
                Console.Write(by);
            }
            Console.WriteLine();
            Commands type = (Commands)bytes[0];
            bytes.RemoveAt(0);
            switch (type)
            {
                case Commands.OutputConsole:
                    ht.ReceiveOutput(bytes);
                    break;
                case Commands.UpdatePositionData:
                    ht.UpdatePosition(bytes);
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

