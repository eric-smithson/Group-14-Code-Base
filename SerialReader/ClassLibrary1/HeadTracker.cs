using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PiTracker
{
    public class HeadTracker
    {
        public static HeadTracker Singleton;

        static HeadTracker()
        {
            // Initialize singleton
            Singleton = new HeadTracker(new PiSerial());
        }

        public class PositionDataEventArgs : EventArgs
        {
            public Vector3 LeftEye;
            public Vector3 RightEye;
            public Boolean LostTracking
            {
                get
                {
                    return float.IsNaN(LeftEye.x)
                        || float.IsNaN(LeftEye.y)
                        || float.IsNaN(LeftEye.z)
                        || float.IsNaN(RightEye.x)
                        || float.IsNaN(RightEye.y)
                        || float.IsNaN(RightEye.z);
                }
            }

            public PositionDataEventArgs(float le_x, float le_y, float le_z,
                float re_x, float re_y, float re_z)
            {
                LeftEye = new Vector3(le_x, le_y, le_z);
                RightEye = new Vector3(re_x, re_y, re_z);
            }
        }

        public class OutputEventArgs : EventArgs
        {
            public String Output;

            public OutputEventArgs(string output)
            {
                Output = output;
            }
        }

        public event EventHandler<PositionDataEventArgs> PositionDataUpdate;
        public event EventHandler<OutputEventArgs> Output;

        public PiReader piReader;


        public HeadTracker(ISerial serial)
        {
            piReader = new PiReader(this, serial);
        }

        public void StartDistortionCalibration(int cameraNumber)
        {
            piReader.WriteCommand(
                PiReader.Commands.CameraDistortionCalibration,
                new List<byte>((byte)cameraNumber));
        }

        public void AddCalibrationPoint(Vector3 point, float[] quater) // quater is size 4
        {
            List<byte> temp1 = new List<byte>();

            temp1.AddRange(BitConverter.GetBytes(point.x));
            temp1.AddRange(BitConverter.GetBytes(point.y));
            temp1.AddRange(BitConverter.GetBytes(point.z));

            byte[,] send = new byte[quater.Length, 4];
            for (int i = 0; i < quater.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(quater[i]);

                for (int j = 0; j < 4; j++)
                    send[i, j] = temp[j];
            }

            for (int i = 0; i < send.Length; i++)
            {
                byte[] row = Enumerable.Range(0, send.Rank)
                    .Select(column => send[i, column]).ToArray();
                temp1.AddRange(row);
            }

            piReader.WriteCommand(PiReader.Commands.AddCalibrationPoint, temp1);
        }

        public void ResetPositionalCalibration()
        {
            piReader.WriteCommand(PiReader.Commands.ResetPositional, new List<byte>());
        }

        public long FavoriteNumber()
        {
            return 0xB00B1E5;
        }

        public void SetEyeDistance(float dis)
        {
            // TODO: Fill in function, call PiReader
            byte[] bdis = BitConverter.GetBytes(dis);

            piReader.WriteCommand(
                PiReader.Commands.SetEyeDistance, 
                new List<byte>(bdis));
        }

        // receives from rpi, triggers event in unity
        public void UpdatePosition(float le_x, float le_y, float le_z,
            float re_x, float re_y, float re_z)
        {
            PositionDataEventArgs pdArgs = new PositionDataEventArgs(le_x, le_y,
                le_z, re_x, re_y, re_z);
            PositionDataUpdate?.BeginInvoke(this, pdArgs, null, null);
        }

        // receives from rpi, triggers event here
        public void ReceiveOutput(string output)
        {
            OutputEventArgs oArgs = new OutputEventArgs(output);
            Output?.BeginInvoke(this, oArgs, null, null);
        }
    }
}
