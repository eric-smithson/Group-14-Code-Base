using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLibrary1
{
    public class HeadTracker
    {
        public event EventHandler<PositionDataEventArgs> PositionDataUpdate;
        void UpdatedPosition(float x, float y, float z)
        {
            PositionDataEventArgs pdea = new PositionDataEventArgs(x, y, z);
            PositionDataUpdate?.BeginInvoke(this, pdea, null, null);
        }

        public void StartDistortionCalibration()
        {

        }

        public void ResetPositionalCalibration()
        {

        }

        public int FavoriteNumber()
        {
            return 666;
        }

        // public void AddCalibrationPoint(Vector3 point)
    }

    public class PositionDataEventArgs : EventArgs
    {
        public float X, Y, Z;
        public PositionDataEventArgs(float x, float y, float z)
        {

            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }

}
