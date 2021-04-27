using System;
using System.Collections.Generic;
using System.Linq;
using covid19_backend.Models;

namespace covid19_backend.Services
{
    public class Prediction
    {
        private static double Polynomial(double[] y) {
            int length = y.Length;
            double[] x = new double[length];
            for(int i = 0; i < length; i++) {
                x[i] = i;
            }
            Func<double, double, double, double, double> operacion = (x, a, b, c) => {
                return (Math.Pow(x, 2) * c) + (b * x) + a;
            };
            var curve = MathNet.Numerics.Fit.Polynomial(x, y, 2, MathNet.Numerics.LinearRegression.DirectRegressionMethod.NormalEquations);
            return operacion(length, curve[0], curve[1], curve[2]);
        }

        public static double GetTotal(List<Vaccination> data, DateTime dayToPredict) {
            double result = 0;
            double[] lastRecords = TotalVaccinations.GetLast(data, dayToPredict.AddDays(-1));

            // Prediction using 90 days
            double prediction1 = Polynomial(lastRecords);
            // Prediction using 28 days
            double prediction2 = Polynomial(lastRecords.Skip(62).ToArray());
            // Prediction using 21 days
            double prediction3 = Polynomial(lastRecords.Skip(69).ToArray());
            // Prediction using 14 days
            double prediction4 = Polynomial(lastRecords.Skip(76).ToArray());
            // Prediction using 10 days
            double prediction5 = Polynomial(lastRecords.Skip(80).ToArray());
            // Prediction using 7 days
            double prediction6 = Polynomial(lastRecords.Skip(83).ToArray());
            // Prediction using 5 days
            double prediction7 = Polynomial(lastRecords.Skip(85).ToArray());

            // Promedio
            result = (prediction1 + prediction2 + prediction3 + prediction4 + prediction5 + prediction6 + prediction7) / 7;
            double previous = TotalVaccinations.GetTotal(data, dayToPredict.AddDays(-1));
            if(previous > result) {
                result = previous;
            }
            return Math.Round(result);
        } 
    }
}