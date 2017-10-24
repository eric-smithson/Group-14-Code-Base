using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ClassLibrary1.Calibration;

namespace PiTrackerTest
{
    [TestClass]
    public class MultilaterizerTest
    {
        System.Random rng = new System.Random();

        void PerformMulterlaterizertTest(Vector3 actual, List<Vector3> measurePoints, float tolerance, float noise = 0)
        {
            if (measurePoints.Distinct().Count() != measurePoints.Count)
                Assert.Inconclusive("Similar points generated");

            IEnumerable<float> distances = measurePoints.Select(l => (l - actual).magnitude + (float) rng.NextDouble() * noise);

            Vector3 calculated = Multilaterizer.GetPointFromDistances(measurePoints, distances);
            float error = (calculated - actual).magnitude;
            Assert.IsTrue(error <= tolerance, "Calcualted outside of tolerance (within {0}). Exp: {1}, Got: {2}",
                error, actual, calculated);
        }

        Vector3 RandomVector3(System.Random rng, int range)
        {
            return new Vector3(
                (float)rng.NextDouble() * range * 2 - range,
                (float)rng.NextDouble() * range * 2 - range,
                (float)rng.NextDouble() * range * 2 - range
            );
        }

        [TestMethod]
        public void MulterlaterizerTest4Locations()
        {
            const float Tolerance = 1e-4f; // 0.1mm 
            Vector3 actual = new Vector3(1, 2, 3);
            List<Vector3> locations = new List<Vector3>()
            {
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, 0, 1)
            };
            PerformMulterlaterizertTest(actual, locations, Tolerance);
        }

        [TestMethod]
        public void MulterlaterizerTest20Locations()
        {
            System.Random rng = new System.Random();
            const float Tolerance = 1e-4f; // 0.1mm 
            Vector3 actual = RandomVector3(rng, 100);
            List<Vector3> locations = Enumerable.Range(1, 20).Select(i => RandomVector3(rng, 100)).ToList();

            PerformMulterlaterizertTest(actual, locations, Tolerance);
        }

        [TestMethod]
        public void MulterlaterizerTestSmallNoise()
        {
            System.Random rng = new System.Random();
            const float Tolerance = 1e-4f; // 0.1mm 
            const float Noise = 1e-5f;
            Vector3 actual = RandomVector3(rng, 2);
            List<Vector3> locations = Enumerable.Range(1, 20).Select(i => RandomVector3(rng, 2)).ToList();

            PerformMulterlaterizertTest(actual, locations, Tolerance, Noise);
        }

        [TestMethod]
        public void MulterlaterizerTestLargeNoise()
        {
            System.Random rng = new System.Random();
            const float Tolerance = 1e-3f; // 1 cm
            const float Noise = 1e-3f; // 1 mm
            Vector3 actual = RandomVector3(rng, 2);
            List<Vector3> locations = Enumerable.Range(1, 100).Select(i => RandomVector3(rng, 2)).ToList();

            PerformMulterlaterizertTest(actual, locations, Tolerance, Noise);
        }

    }
}
