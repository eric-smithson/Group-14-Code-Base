using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Management;

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
<<<<<<< Updated upstream
                string piPortName = null;
                using (var searcher = new ManagementObjectSearcher
                    ("SELECT * FROM WIN32_SerialPort"))
                {
                    string[] portnames = SerialPort.GetPortNames();
                    var ports = searcher.Get().Cast<ManagementBaseObject>().ToList();

                    // Pi Ports (Ports we can access from SerialPort, and start with "PI USB to Serial"
                    var piPorts = ports.Where(p => portnames.Contains<string>(p["DeviceID"].ToString())
                    && p["Caption"].ToString().StartsWith("PI USB to Serial"));

                    // Set 
                    if (piPorts.Count() > 0)
                        piPortName = piPorts.First()["DeviceID"].ToString();
                }

                com.PortName = piPortName ?? throw new Exception("Could not find any connected Pis");
=======
                com.PortName = "COM7";
>>>>>>> Stashed changes
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
                    com?.Close();
                    com?.Dispose();
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
