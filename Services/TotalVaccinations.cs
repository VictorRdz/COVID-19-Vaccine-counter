using System;
using System.Collections.Generic;
using covid19_backend.Models;

namespace covid19_backend.Services
{
    public class TotalVaccinations
    {
        public static double GetTotal(List<Vaccination> data, DateTime date) {
            string stringDate = date.ToString("yyyy-MM-dd");
            double total = 0;
            foreach (var item in data)
            {
                string itemLocation = item.location;
                string itemValue = item.total_vaccinations;
                string itemDate = item.date;
                if(itemValue != "" && itemDate == stringDate && itemLocation == "World") {
                    total += double.Parse(itemValue);
                }
            }
            return total;
        }

        public static double[] GetLast(List<Vaccination> data, DateTime lastDay) {
            // Get last 90 vaccination records
            int total = 90;
            double[] result = new double[total];
            for (int i = 0; i < total; i++)
            {
                DateTime thisDate = lastDay.AddDays(-i);
                result[total - 1 - i] = GetTotal(data, thisDate);
            }
            return result;
        }
    }
}