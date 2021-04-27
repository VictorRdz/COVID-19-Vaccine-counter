using System;
using System.Collections.Generic;
using covid19_backend.Models;

namespace covid19_backend.Services
{
    public class Error
    {
        public static void PrintTotalError(List<Vaccination> data) {
            DateTime baseDate = DateTime.Now.AddDays(-1);
            int totalDays = 10;
            double[] errorArray = new double[totalDays];

            for(int i = 0; i < totalDays; i++) {
                baseDate = baseDate.AddDays(-i);
                double actual = TotalVaccinations.GetTotal(data, baseDate);
                double prediction = Prediction.GetTotal(data, baseDate);
                double error = 100 / actual * prediction - 100;
                errorArray[i] = error;
                Console.WriteLine("Error el " + baseDate.ToString("yyyy-MM-dd") + ": " + string.Format("{0:N2}%", error));
            }
            double result = 0;
            foreach (var error in errorArray)
            {
                result += Math.Abs(error);
            }
            result = result / totalDays;
            Console.WriteLine("Error promedio en " + totalDays + " dias: " + string.Format("{0:N2}%", result));
        }

        public static void PrintError(List<Vaccination> data) {
            DateTime baseDate = DateTime.Now.AddDays(-1);
            int totalDays = 10;
            double[] errorArray = new double[totalDays];

            for(int i = 0; i < totalDays; i++) {
                baseDate = baseDate.AddDays(-i);
                double previous = TotalVaccinations.GetTotal(data, baseDate.AddDays(-1));
                double actual = TotalVaccinations.GetTotal(data, baseDate);
                double prediction = Prediction.GetTotal(data, baseDate);

                double expected = actual - previous;
                double predicted = prediction - previous;

                double error = 100 / expected * predicted - 100;
                Console.WriteLine("Error el " + baseDate.ToString("yyyy-MM-dd") + ": " + string.Format("{0:N2}%", error));
                errorArray[i] = error;
            }
            double result = 0;
            foreach (var error in errorArray)
            {
                result += Math.Abs(error);
            }
            result = result / totalDays;
            Console.WriteLine("Error promedio en " + totalDays + " dias: " + string.Format("{0:N2}%", result));
        }
        
        public static string GetDataError(List<Vaccination> data, DateTime date) {
            double actual = TotalVaccinations.GetTotal(data, date);
            double prediction = Prediction.GetTotal(data, date);
            double error = 100 / actual * prediction - 100;
            return string.Format("{0:N2}%", error);
        }

        public static string GetError(List<Vaccination> data, DateTime date) {
            double previous = TotalVaccinations.GetTotal(data, date.AddDays(-1));
            double actual = TotalVaccinations.GetTotal(data, date);
            double prediction = Prediction.GetTotal(data, date);

            double expected = actual - previous;
            double predicted = prediction - previous;

            double error = 100 / expected * predicted - 100;
            return string.Format("{0:N2}%", error);
        }
    }
}