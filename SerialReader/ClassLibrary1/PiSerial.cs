using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace PiTracker
{
    public class PiSerial : ISerial, IDisposable
    {
        SerialPort com;

        private enum COMSettings
        {
            Virtual,
            RPI,
        }

        public PiSerial()
        {
            OpenCom();
        }

        private void OpenCom()
        {
            Debug.WriteLine("Setting up serial reader");

            com = makeCOM(COMSettings.RPI);

            com.Open();

            Debug.WriteLine(com);
        }
        
        private SerialPort makeCOM(COMSettings s)
        {
            com = new SerialPort();
            if (s == COMSettings.RPI)
            {
                com.PortName = "COM6";
                com.BaudRate = 115200;
                com.DataBits = 8;
                com.Parity = Parity.None;
                com.StopBits = StopBits.One;

            }
            if (s == COMSettings.Virtual)
            {
                com.PortName = "COM4";
                com.BaudRate = 115200;
                com.DataBits = 8;
                com.Parity = Parity.None;
                com.StopBits = StopBits.One;
            }

            return com;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            if (!com.IsOpen)
                return 0;

            return com.Read(buffer, offset, count);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            if (!com.IsOpen)
                return;

            com.Write(buffer, offset, count);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    com.Dispose();
                }

                disposedValue = true;
            }
        }

        // ~PiSerial() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
