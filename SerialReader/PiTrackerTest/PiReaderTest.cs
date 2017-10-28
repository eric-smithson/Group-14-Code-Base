using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using PiTracker;
using PiTrackerTest.Mocks;
using UnityEngine;
using System.Threading;
using Consistent_Overhead_Byte_Stuffing;
using System.Linq;
using System.Text;


namespace PiTrackerTest
{
    [TestClass]
    public class PiReaderTest
    {
        [TestMethod]
        public void TestPositionUpdate()
        {
            MockSerial serial = new MockSerial();
            HeadTracker tracker = new HeadTracker(serial);
            var receivedEvents = new List<HeadTracker.PositionDataEventArgs>();
            tracker.PositionDataUpdate += (sender, e) =>
            {
                receivedEvents.Add(e);
            };

            Vector3 leftEye = new Vector3(1, 2, 3);
            Vector3 rightEye = new Vector3(4, 5, 6);
            List<byte> sendBytes = new List<byte>();
            sendBytes.Add((byte) PiReader.Commands.UpdatePositionData);
            sendBytes.AddRange(BitConverter.GetBytes(leftEye.x));
            sendBytes.AddRange(BitConverter.GetBytes(leftEye.y));
            sendBytes.AddRange(BitConverter.GetBytes(leftEye.z));
            sendBytes.AddRange(BitConverter.GetBytes(rightEye.x));
            sendBytes.AddRange(BitConverter.GetBytes(rightEye.y));
            sendBytes.AddRange(BitConverter.GetBytes(rightEye.z));
            sendBytes = COBS.Encode(sendBytes).ToList();
            sendBytes.Add(0);

            Assert.AreEqual(0, receivedEvents.Count);

            serial.GiveReadBytes(sendBytes.ToArray());

            Thread.Sleep(50); // Wait 50 ms

            Assert.AreEqual(1, receivedEvents.Count);
            Assert.AreEqual(leftEye, receivedEvents[0].LeftEye);
            Assert.AreEqual(rightEye, receivedEvents[0].RightEye);
        }

        [TestMethod]
        public void TestOutput()
        {
            MockSerial serial = new MockSerial();
            HeadTracker tracker = new HeadTracker(serial);
            var outputEvents = new List<HeadTracker.OutputEventArgs>();
            tracker.Output += (sender, e) =>
            {
                outputEvents.Add(e);
            };

            string expected = "testing, 123";
            List<byte> sendBytes = new List<byte>();
            sendBytes.Add((byte)PiReader.Commands.OutputConsole);
            sendBytes.AddRange(ASCIIEncoding.ASCII.GetBytes(expected));
            sendBytes = COBS.Encode(sendBytes).ToList();
            sendBytes.Add(0);

            Assert.AreEqual(0, outputEvents.Count);

            serial.GiveReadBytes(sendBytes.ToArray());

            Thread.Sleep(50); // Wait 50 ms

            Assert.AreEqual(1, outputEvents.Count);
            Assert.AreEqual(expected, outputEvents[0].Output);
        }

        [TestMethod]
        public void TestSetEyeDistance()
        {
            MockSerial serial = new MockSerial();
            HeadTracker tracker = new HeadTracker(serial);
            var writtenEvents = new List<MockSerial.BytesWrittenEventArgs>();
            serial.OnBytesWritten += (sender, e) =>
            {
                writtenEvents.Add(e);
            };

            float expected = 50.0f;
            /*List<byte> expectedBytes = new List<byte>();
            sendBytes.Add((byte)PiReader.Commands.);
            sendBytes.AddRange(ASCIIEncoding.ASCII.GetBytes(expected));
            sendBytes = COBS.Encode(sendBytes).ToList();
            sendBytes.Add(0);*/

            Assert.AreEqual(0, writtenEvents.Count);

            tracker.SetEyeDistance(expected);

            Assert.AreEqual(1, writtenEvents.Count);

            Assert.Inconclusive("TODO: Need to compare written bytes with expected");
        }
    }
}
