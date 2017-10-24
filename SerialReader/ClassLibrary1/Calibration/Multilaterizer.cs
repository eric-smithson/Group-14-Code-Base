
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MathNet.Numerics;
using MathNet.Numerics.Optimization;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Diagnostics;
using MathNet.Numerics.LinearAlgebra.Factorization;

namespace ClassLibrary1.Calibration
{
    public static class Multilaterizer
    {
         public static Vector3 GetPointFromDistances(IEnumerable<Vector3> points, IEnumerable<float> distancePerPoint)
        {

            IEnumerable<Vector<double>> cPoints = points.Select(v => CreateVector.Dense<double>(new double[] { v.x, v.y, v.z }));
            Vector<double> dis = CreateVector.Dense(distancePerPoint.Select(x => (double) x).ToArray());

            /*Tuple<Vector3, Vector3> vGuess = DetermineInitialEstimates(points.Take(3).ToList(), distancePerPoint.Take(3).ToList());
            Vector<double> v1 = CreateVector.Dense(new double[] { vGuess.Item1.x, vGuess.Item1.y, vGuess.Item1.z });
            Vector<double> v2 = CreateVector.Dense(new double[] { vGuess.Item2.x, vGuess.Item2.y, vGuess.Item2.z });
            Vector <double> estimate = calculateError(v1, cPoints, dis) <= calculateError(v2, cPoints, dis) ? v1 : v2;
            double error = calculateError(estimate, cPoints, dis);*/

            /*for (int i = 0; i < 1000; i++)
            {
                estimate = newtonIteration(estimate, cPoints, dis);

                error = calculateError(estimate, cPoints, dis);
            }*/

            Vector<double> estimate = method2(cPoints, dis);

            return new Vector3((float)estimate[0], (float)estimate[1], (float)estimate[2]);
        }

        static float Sqr(float x)
        {
            return x * x;
        }

        /*
        static Tuple<Vector3, Vector3> DetermineInitialEstimates(List<Vector3> points, List<float> distancePerPoint)
        {
            Vector3 e_x = (points[1] - points[0]).normalized;
            float i = Vector3.Dot(e_x, (points[2] - points[0]));
            Vector3 e_y = (points[2] - points[0] - i * e_x).normalized;
            Vector3 e_z = Vector3.Cross(e_x, e_y);
            float d = (points[1] - points[0]).magnitude;
            float j = Vector3.Dot(e_y, points[2] - points[0]);

            float x = (Sqr(distancePerPoint[0]) - Sqr(distancePerPoint[1]) + Sqr(d)) / (2 * d);
            float y = (Sqr(distancePerPoint[0]) - Sqr(distancePerPoint[2]) + Sqr(i) + Sqr(j)) / (2 * j);
            float z = (float)Math.Sqrt(Sqr(distancePerPoint[0]) - Sqr(x) - Sqr(y));
            if (double.IsNaN(z))
                z = 0;

            Vector3 resultSubZ = points[0] + x * e_x + y * e_y;

            return new Tuple<Vector3, Vector3>(resultSubZ + z * e_z, resultSubZ - z *e_z);
        }

        static double calculateError(Vector<double> estimate, IEnumerable<Vector<double>> points, Vector<double> dis)
        {
            int m = dis.Count;

            // Mean square error
            Vector<double> distancesFromEstimate = CreateVector.Dense<double>(m, i => (double)(estimate - points.ElementAt(i)).L2Norm());
            Vector<double> r = dis - distancesFromEstimate;

            return r.PointwisePower(2).Sum();
        }

        static Vector<double> newtonIteration(Vector<double> estimate, IEnumerable<Vector<double>> points, Vector<double> dis)
        {
            int m = dis.Count;
            int n = 3;
            Vector<double> distancesFromEstimate = CreateVector.Dense<double>(m, i => (double) (estimate - points.ElementAt(i)).L2Norm());
            Vector<double> r = dis - distancesFromEstimate;

            Matrix<double> jacobian = CreateMatrix.Dense<double>(m, n, (i, j) => -(estimate[j] - points.ElementAt(i)[j]) / distancesFromEstimate[i]);

            // Gauss-Netwon non-linear least squares algorithm
            Vector<double> newEstimate = estimate - (jacobian.Transpose() * jacobian).Inverse() * jacobian.Transpose() * r;
            
            if ((jacobian.Transpose() * jacobian).Determinant() == 0)
                return newEstimate;

            if (newEstimate.Any(a => double.IsNaN(a)))
                return newEstimate;

            return newEstimate;
        }*/

        static Vector<double> method2(IEnumerable<Vector<double>> points, Vector<double> dis)
        {
            Matrix<double> abc = CreateMatrix.Dense<double>(dis.Count - 1, 3);
            Vector<double> d = CreateVector.Dense<double>(dis.Count - 1);
            for (int i = 1; i < dis.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                    abc[i - 1, j] = 2 * (points.ElementAt(i)[j] - points.ElementAt(0)[j]);

                d[i - 1] = dis[0] * dis[0] - dis[i] * dis[i]
                    + (points.ElementAt(i) - points.ElementAt(0)).PointwisePower(2).Sum();
            }
            Vector<double> x = CreateVector.Dense<double>(3);
            try
            {
                // compute the SVD
                Svd<double> svd = abc.Svd(true);

                int m = abc.RowCount;
                int n = abc.ColumnCount;

                // get matrix of left singular vectors with first n columns of U
                Matrix<double> U1 = svd.U.SubMatrix(0, m, 0, n);
                // get matrix of singular values
                Matrix<double> S = CreateMatrix.Diagonal(n, n, svd.S.ToArray());
                // get matrix of right singular vectors
                Matrix<double> V = svd.VT.Transpose();

                x = V.Multiply(S.Inverse()).Multiply(U1.Transpose().Multiply(d));
            }
            catch (Exception e)
            {
                Debugger.Break();
            }
            return x + points.ElementAt(0);
        }
    }
}
