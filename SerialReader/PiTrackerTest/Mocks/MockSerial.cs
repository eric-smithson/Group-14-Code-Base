using PiTracker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PiTrackerTest.Mocks
{
    class MockSerial : ISerial
    {
        public List<byte> readByteBuffer = new List<byte>();

        public class BytesWrittenEventArgs : EventArgs
        {
            public byte[] Bytes;
            public BytesWrittenEventArgs(byte[] bytes)
            {
                Bytes = (byte[]) bytes.Clone();
            }
        }

        public event EventHandler<BytesWrittenEventArgs> OnBytesWritten;

        public int Read(byte[] buffer, int offset, int count)
        {
            // Make sure we are not requesting more than the current buffer can supply
            while (readByteBuffer.Count < count)
            {
                Thread.Sleep(0);
            }

            // Copy data to buffer
            readByteBuffer.Take(count).ToArray().CopyTo(buffer, offset);
            readByteBuffer = readByteBuffer.Skip(count).ToList();


            return count;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            // Copy written bytes to eventargs
            BytesWrittenEventArgs written = new BytesWrittenEventArgs(
                buffer.Skip(offset).Take(count).ToArray());

            OnBytesWritten?.Invoke(this, written);
        }

        public void GiveReadBytes(byte[] bytesToRead)
        {
            readByteBuffer.AddRange(bytesToRead);
        }

        public Exception OpenCom(string COMName, HeadTracker.COMSettings s)
        {
            throw new NotImplementedException();
        }
    }
}
