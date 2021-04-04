using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using covid19_backend.Data;
using covid19_backend.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace covid19_backend.Tasks
{
    public class UpdateData : IHostedService, IDisposable {
        private readonly ILogger<UpdateData> logger;
        private Timer dataUpdater;
        private Timer dayTimer;
        private static double displayValue = 0;
        private static double yesterdayError = 0;
        private List<Vaccination> data = new List<Vaccination>();
        private int hourToUpdate = 11;

        public UpdateData(ILogger<UpdateData> logger) 
        {
            this.logger = logger;
        }

        public void Dispose()
        {
            dataUpdater?.Dispose();
            dayTimer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {            
            UpdateDataFromServer(true);
            dayTimer = new Timer(o => UpdateDataFromServer()
                , null, TimeSpan.Zero, TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Se detuvo");
            return Task.CompletedTask;
        }

        public static double GetDisplayValue()
        {
            return Math.Round(displayValue);
        }

        public static double GetYesterdayError()
        {
            return yesterdayError;
        }

        public async void UpdateDataFromServer(bool forceUpdate = false) 
        {
            DateTime dateBase = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 11:00 AM");
            DateTime dateNow = DateTime.Now;
            if((DateTime.Now.Hour == hourToUpdate) || forceUpdate ) {
                logger.LogInformation(DateTime.Now.ToString("dd-MM-yy - h:mm tt => ") + "Updating data from server...");
                // Update data from server.
                bool updated = false;
                updated = await VaccinationData.DownloadNewData();
                if(updated) {
                    logger.LogInformation(DateTime.Now.ToString("dd-MM-yy - h:mm tt => ") + "Downloaded data from server.");
                    bool upToDate = false;
                    data = await VaccinationData.TransformDataToCsv();
                    if(dateNow < dateBase) {
                        upToDate = VaccinationData.IsDataUpdated(data, dateNow.AddDays(-2));
                    }
                    else {
                        upToDate = VaccinationData.IsDataUpdated(data, dateNow.AddDays(-1));
                    }
                    if(!upToDate) {
                        if(hourToUpdate <= 23) {
                            hourToUpdate++;
                            logger.LogWarning(DateTime.Now.ToString("dd-MM-yy - h:mm tt => ") + "Server data is NOT up-to-date, trying again at " + hourToUpdate + ":" + DateTime.Now.ToString("mm"));

                        }
                        else {
                            hourToUpdate = 0;
                            logger.LogWarning(DateTime.Now.ToString("dd-MM-yy - h:mm tt => ") + "Server data is NOT up-to-date, trying again at 00:" + DateTime.Now.ToString("mm"));
                        }
                    }
                    else {
                        logger.LogInformation(DateTime.Now.ToString("dd-MM-yy - h:mm tt => ") + "Server data is up-to-date.");
                        InitializeData();
                    }
                }
            }
        }

        public void InitializeData()
        {

            DateTime dateBase = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 11:00 AM");
            DateTime dateNow = DateTime.Now;

            if(dateBase > dateNow) {
                dateBase = dateBase.AddDays(-1);
            }

            double prediction = VaccinationData.PredictVaccination(data, dateBase.ToString("yyyy-MM-dd"));

            int timeInSeconds = (dateNow - dateBase).Hours * 3600 + (dateNow - dateBase).Minutes * 60 + (dateNow - dateBase).Seconds;
            
            double initialValue = displayValue;
            if(initialValue == 0) {
                initialValue = VaccinationData.GetTotalVaccinations(data, "World", dateBase.AddDays(-1).ToString("yyyy-MM-dd"));
                initialValue = RandomizeData.GetInitialValue(initialValue, prediction, timeInSeconds);
                displayValue = initialValue;
            }

            yesterdayError = VaccinationData.GetSinglePredictionError(data, dateBase.AddDays(-1).ToString("yyyy-MM-dd"));
            if(Math.Abs(yesterdayError) > 5) {
                logger.LogWarning(DateTime.Now.ToString("dd-MM-yy - h:mm tt => ") + "Yesterday error was: " + yesterdayError);
            }
            else {
                logger.LogInformation(DateTime.Now.ToString("dd-MM-yy - h:mm tt => ") + "Yesterday error was: " + yesterdayError + "%");
            }
            logger.LogInformation(DateTime.Now.ToString("dd-MM-yy - h:mm tt => ") + "Today's prediction is: " + prediction + ", starting at " + displayValue);
            dataUpdater?.Dispose();
            // Update data every second
            logger.LogInformation(DateTime.Now.ToString("dd-MM-yy - h:mm tt => ") + "Restarting data randomizer.");
            dataUpdater = new Timer(o =>
            {
                displayValue += RandomizeData.GetUpdateRandom(initialValue, prediction);
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

        }


    }

}