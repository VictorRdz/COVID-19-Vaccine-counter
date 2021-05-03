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
                string url = "https://covid.ourworldindata.org/data/vaccinations/vaccinations.csv";
                string filePath = "wwwroot/";
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
            string url = "wwwroot/";
            string filename = "data.csv";
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