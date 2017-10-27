using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using PiTracker;
using PiTrackerTest.Mocks;
using UnityEngine;
using System.Threading;
using Consistent_Overhead_Byte_Stuffing;
using System.Linq;

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
            Assert.AreEqual(leftEye, receivedEvents[0].RightEye);
        }
    }
}
