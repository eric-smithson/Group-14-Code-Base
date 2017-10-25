using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace ClassLibrary1
{
    public class PiSerial : ISerial, IDisposable
    {
        SerialPort com;

        public PiSerial()
        {
            OpenCom();
        }

        private void OpenCom()
        {
            Debug.WriteLine("Setting up serial reader");

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

            com.Open();

            Debug.WriteLine(com);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return com.Read(buffer, offset, count);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
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
