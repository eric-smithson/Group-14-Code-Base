using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PiTracker
{
    public interface ISerial
    {
        int Read(byte[] buffer, int offset, int count);
        void Write(byte[] buffer, int offset, int count);

        Exception OpenCom(string COMName, HeadTracker.COMSettings s);
    }
}
