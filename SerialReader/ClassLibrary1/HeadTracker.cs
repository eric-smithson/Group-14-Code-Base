﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PiTracker
{
    public class HeadTracker
    {
        public static HeadTracker Singleton;

        public enum COMSettings
        {
            Virtual,
            RPI,
        }

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
        public ISerial serial;

        private HeadTracker(ISerial serial)
        {
            piReader = new PiReader(this, serial);
            this.serial = serial;
        }

        public void ResetCalibration(int cameraNumber)
        {
            piReader.WriteCommand(
                PiReader.Commands.ResetCalibration,
                new List<byte>((byte)cameraNumber));
        }

        public void AddCalibration(Vector3 point, Quaternion rot, float checkerSize, int checkerRows, int checkerColumns) // quater is size 4
        {
            List<byte> b_command = new List<byte>();

            b_command.AddRange(BitConverter.GetBytes(point.x));
            b_command.AddRange(BitConverter.GetBytes(point.y));
            b_command.AddRange(BitConverter.GetBytes(point.z));

            b_command.AddRange(BitConverter.GetBytes(rot.w));
            b_command.AddRange(BitConverter.GetBytes(rot.x));
            b_command.AddRange(BitConverter.GetBytes(rot.y));
            b_command.AddRange(BitConverter.GetBytes(rot.z));

            b_command.AddRange(BitConverter.GetBytes(checkerSize));
            b_command.AddRange(BitConverter.GetBytes(checkerRows));
            b_command.AddRange(BitConverter.GetBytes(checkerColumns));

            piReader.WriteCommand(PiReader.Commands.AddCalibration, b_command);
        }

        public long FavoriteNumber()
        {
            return 0xB00B1E5;
        }

        public void SetEyeDistance(float dis)
        {
            byte[] bdis = BitConverter.GetBytes(dis);

            piReader.WriteCommand(
                PiReader.Commands.SetEyeDistance, 
                new List<byte>(bdis));
        }

        // receives from rpi, triggers event in unity
        public void UpdatePosition(List<byte> bytes)

        {
            if (bytes.Count != 24) {
                System.Diagnostics.Debug.WriteLine("Incorrect amount of bytes sent");
            }
            float le_x, le_y, le_z, re_x, re_y, re_z;
            byte[] b_arr = bytes.ToArray();

            le_x = BitConverter.ToSingle(b_arr, 0);
            le_y = BitConverter.ToSingle(b_arr, 4);
            le_z = BitConverter.ToSingle(b_arr, 8);
            re_x = BitConverter.ToSingle(b_arr, 12);
            re_y = BitConverter.ToSingle(b_arr, 16);
            re_z = BitConverter.ToSingle(b_arr, 20);

            PositionDataEventArgs pdArgs = new PositionDataEventArgs(le_x, le_y, le_z, re_x, re_y, re_z);
            PositionDataUpdate?.BeginInvoke(this, pdArgs, null, null);
        }

        public void ReceiveOutput(List<byte> bytes)
        {
            string output = System.Text.Encoding.ASCII.GetString(bytes.ToArray());
            OutputEventArgs oArgs = new OutputEventArgs(output);
            Output?.BeginInvoke(this, oArgs, null, null);
        }
    }
}
