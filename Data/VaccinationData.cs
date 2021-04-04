using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using covid19_backend.Models;
using covid19_backend.Tasks;
using CsvHelper;
using MathNet.Numerics;
using Microsoft.Extensions.Logging;

namespace covid19_backend.Data
{
    public class VaccinationData {
        public static async Task<bool> DownloadNewData() {
            try
            {
                var client = new WebClient();
                string url = "https://raw.githubusercontent.com/owid/covid-19-data/master/public/data/vaccinations/vaccinations.csv";
                string filePath = "Data/Source/";
                string filename = "test.csv";
                await Task.Run(() => client.DownloadFile(url, filePath + filename));
                return true;
            }
            catch (WebException) 
            {              
                return false;  
            }
        }

        public static async Task<List<Vaccination>> TransformDataToCsv() {
            string url = "Data/Source/";
            string filename = "test.csv"; //temporal
            IEnumerable<Vaccination> result = Enumerable.Empty<Vaccination>();
            
            await Task.Run(() => {
                var reader = new StreamReader(url + filename);
                var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                result = csv.GetRecords<Vaccination>();
            });
                        
            return result.ToList();
        }

        public static double GetTotalVaccinations(List<Vaccination> data, string location, string date) {
            double total = 0;
            foreach (var item in data)
            {
                string itemLocation = item.location;
                string itemValue = item.total_vaccinations;
                string itemDate = item.date;
                if(itemValue != "" && itemDate == date && itemLocation == location) {
                    total += double.Parse(itemValue);
                }
            }
            return total;
        }

        public static bool IsDataUpdated(List<Vaccination> data, DateTime lastDay) {
            double value = VaccinationData.GetTotalVaccinations(data, "World", lastDay.ToString("yyyy-MM-dd"));
            if(value > 0) {
                return true;
            }
            else {
                return false;
            }
        }

        public static double PredictNext(double[] y) {
            double[] x = {0, 1, 2, 3, 4};
            Func<double, double, double, double, double> operacion = (x, a, b, c) => {
                return (Math.Pow(x, 2) * a) + (b * x) + c;
            };
            var curve = MathNet.Numerics.Fit.Polynomial(x, y, 2, MathNet.Numerics.LinearRegression.DirectRegressionMethod.NormalEquations);

            double val = operacion(5, curve[2], curve[1], curve[0]);
            return val;
        }


        public static double[] GetLastFiveTotalVaccination(List<Vaccination> data, DateTime lastDay) {
            // Get last five Vaccination records
            double[] lastFiveVaccination = {0, 0, 0, 0, 0};
            int timeAux = -4; // from day -4 to day 0
            for (int i = 0; i < 5; i++)
            {
                DateTime thisDate = lastDay.AddDays(timeAux);
                lastFiveVaccination[i] = VaccinationData.GetTotalVaccinations(data, "World", thisDate.ToString("yyyy-MM-dd"));
                timeAux ++;
            }
            return lastFiveVaccination;
        }

        public static string TestMeanPredictionError(List<Vaccination> data, string dateFrom, string dateTo) {
            DateTime baseDay = Convert.ToDateTime(dateFrom);
            DateTime lastDay = Convert.ToDateTime(dateTo).AddDays(1);
            List<double> errorValues = new List<double>();

            while(baseDay.ToString("yyyy-MM-dd") != lastDay.ToString("yyyy-MM-dd")) {
                double prediction = VaccinationData.PredictVaccination(data, baseDay.ToString("yyyy-MM-dd"));
                double actualValue = VaccinationData.GetTotalVaccinations(data, "World", baseDay.ToString("yyyy-MM-dd"));
                double errorPercentage = ((100 / actualValue) * prediction) - 100;
                errorValues.Add(errorPercentage);
                baseDay = baseDay.AddDays(1);
            }

            double errorSum = 0;
            foreach (var item in errorValues)
            {
                errorSum += item;
            }
            double meanError = errorSum / errorValues.Count;
            return "Error promedio es " + meanError + "%";
        }

        public static string GetLargestPredictionError(List<Vaccination> data, string dateFrom, string dateTo) {
            DateTime baseDay = Convert.ToDateTime(dateFrom);
            DateTime lastDay = Convert.ToDateTime(dateTo).AddDays(1);
            List<double> errorValues = new List<double>();
            List<string> dateValues = new List<string>();

            while(baseDay.ToString("yyyy-MM-dd") != lastDay.ToString("yyyy-MM-dd")) {
                double prediction = VaccinationData.PredictVaccination(data, baseDay.ToString("yyyy-MM-dd"));
                double actualValue = VaccinationData.GetTotalVaccinations(data, "World", baseDay.ToString("yyyy-MM-dd"));
                double errorPercentage = ((100 / actualValue) * prediction) - 100;
                errorValues.Add(errorPercentage);
                dateValues.Add(baseDay.ToString("yyyy-MM-dd"));
                baseDay = baseDay.AddDays(1);
            }

            double maxError = 0;
            string maxDate = "";
            for (int i = 0; i < errorValues.Count; i++)
            {
                double thisError = errorValues[i];
                if(Math.Abs(thisError) >= Math.Abs(maxError)){
                    maxError = thisError;
                    maxDate = dateValues[i];
                }
            }
            return "Error mas grande es de " + maxError + "% el " + maxDate;
        }

        public static List<string> GetPredictionErrorList(List<Vaccination> data, string dateFrom, string dateTo) {
            DateTime baseDay = Convert.ToDateTime(dateFrom);
            DateTime lastDay = Convert.ToDateTime(dateTo).AddDays(1); // include dateTo day
            List<string> errorValues = new List<string>();

            double previousPrediction = 0;
            double previousValue = 0;
            while(baseDay.ToString("yyyy-MM-dd") != lastDay.ToString("yyyy-MM-dd")) {
                double prediction = VaccinationData.PredictVaccination(data, baseDay.ToString("yyyy-MM-dd"));
                double actualValue = VaccinationData.GetTotalVaccinations(data, "World", baseDay.ToString("yyyy-MM-dd"));
                double errorPercentage = ((100 / actualValue) * prediction) - 100;
                string test = "";
                if(previousPrediction >= prediction) {
                    test += "$$$$$$$$$$";
                }
                if(prediction <= previousValue) {
                    test += "@@@@@@@@@@";
                }
                errorValues.Add(baseDay.ToString("yyyy-MM-dd") + ": " + errorPercentage + "% --- Predict: " + prediction + " | Actual: " + actualValue + test);
                previousPrediction = prediction;
                previousValue = actualValue;
                baseDay = baseDay.AddDays(1);
            }
            return errorValues;
        }

        public static double PredictVaccination(List<Vaccination> data, string dateToPredict) {
            DateTime date = Convert.ToDateTime(dateToPredict);
            DateTime actualDate = date.AddDays(-1);
            DateTime previousDate = actualDate.AddDays(-1);
            // Get actual prediction
            double[] lastFiveVaccination = VaccinationData.GetLastFiveTotalVaccination(data, actualDate);
            double actualPrediction = VaccinationData.PredictNext(lastFiveVaccination) * 0.97;

            // Get actual (real) value for previous prediction
            double previousValue = lastFiveVaccination[4];

            // Get previous prediction (recursive)
            lastFiveVaccination = VaccinationData.GetLastFiveTotalVaccination(data, previousDate);
            double previousPrediction = 0;
            if(date <= Convert.ToDateTime("2020-12-13")) {
                previousPrediction = VaccinationData.PredictNext(lastFiveVaccination) * 0.97;
            }
            else {
                previousPrediction = VaccinationData.PredictVaccination(data, previousDate.ToString("yyyy-MM-dd"));
            }

            // Adjust prediction when previous prediction was big
            // double previousErrorPercentage = ((100 / previousValue) * previousPrediction) - 100;
            // if(Math.Abs(previousErrorPercentage) < 5) {
                // double gap = actualPrediction - previousValue;
                // actualPrediction = previousValue + (gap / 3);
            // }

            // Adjust prediction when previous value is larger
            if(previousValue >= actualPrediction) {
                double gap = previousValue - actualPrediction;
                actualPrediction = previousValue + gap;
            }

            // Make actual prediction larger than previous prediction
            if(actualPrediction < previousPrediction) {
                double gap = previousPrediction - actualPrediction;
                actualPrediction = previousPrediction + (gap / 2);
            }

            return Math.Round(actualPrediction);
        }

        public static double GetSinglePredictionError(List<Vaccination> data, string dateString) {
            double prediction = VaccinationData.PredictVaccination(data, dateString);
            double actualValue = VaccinationData.GetTotalVaccinations(data, "World", dateString);
            double errorPercentage = ((100 / actualValue) * prediction) - 100;
            return errorPercentage;
        }
    }
    
}