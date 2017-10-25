using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace ClassLibrary1
{
    public class PiSerial : ISerial
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
    }
}
