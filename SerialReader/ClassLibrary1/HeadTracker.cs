using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ClassLibrary1
{
    public class HeadTracker
    {
        public static HeadTracker Singleton;

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

        public void StartDistortionCalibration(int cameraNumber)
        {

        }

        public void AddCalibrationPoint(Vector3 point, float[] quater)
        {

        }

        public void ResetPositionalCalibration()
        {

        }

        public long FavoriteNumber()
        {
            return 0xDEADBEEF;
        }

        public void SetEyeDistance(float dis)
        {

        }

        void UpdatePosition(float le_x, float le_y, float le_z,
        float re_x, float re_y, float re_z)
        {
            PositionDataEventArgs pdArgs = new PositionDataEventArgs(le_x, le_y,
                le_z, re_x, re_y, re_z);
            PositionDataUpdate?.BeginInvoke(this, pdArgs, null, null);
        }
    }
}
