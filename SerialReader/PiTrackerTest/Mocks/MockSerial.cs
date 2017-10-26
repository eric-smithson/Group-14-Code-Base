using PiTracker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiTrackerTest.Mocks
{
    class MockSerial : ISerial
    {
        public byte[] readByteBuffer = new byte[0];

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
            int bytesToRead = Math.Min(readByteBuffer.Length, count);

            // Copy data to buffer
            buffer = new byte[bytesToRead];
            readByteBuffer.CopyTo(buffer, 0);

            return bytesToRead;
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
            // Resize readByteBuffer and append new data
            byte[] temp = new byte[bytesToRead.Length + readByteBuffer.Length];
            readByteBuffer.CopyTo(temp, 0);
            bytesToRead.CopyTo(temp, readByteBuffer.Length);
            readByteBuffer = temp;
        }
    }
}
