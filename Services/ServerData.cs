using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using covid19_backend.Models;
using CsvHelper;

namespace covid19_backend.Services
{
    public static class ServerData
    {
        public static async Task<bool> Update() {
            try
            {
                var client = new WebClient();
                string url = "https://raw.githubusercontent.com/owid/covid-19-data/master/public/data/vaccinations/vaccinations.csv";
                string filePath = "";
                string filename = "data.csv";
                await Task.Run(() => client.DownloadFile(url, filePath + filename));
                return true;
            }
            catch (WebException) 
            {              
                return false;  
            }
        }
        public static bool IsUpdated(List<Vaccination> data, System.DateTime date) {
            bool updated = false;
            double total = TotalVaccinations.GetTotal(data, date, TotalVaccinations.TOTAL);
            if(total > 0) {
                updated = true;
            } 
            return updated;
        }

        public static async Task<List<Vaccination>> GetList() {
            string url = "";
            string filename = "data.csv"; //temporal
            IEnumerable<Vaccination> result = Enumerable.Empty<Vaccination>();
            
            await Task.Run(() => {
                var reader = new StreamReader(url + filename);
                var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                result = csv.GetRecords<Vaccination>();
            });
                        
            return result.ToList();
        }
    }
}