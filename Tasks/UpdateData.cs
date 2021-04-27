using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using covid19_backend.Data;
using covid19_backend.Models;
using covid19_backend.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace covid19_backend.Tasks
{
    public class UpdateData : IHostedService, IDisposable {
        private readonly ILogger<UpdateData> logger;
        private Timer dataUpdater;
        private Timer dayTimer;

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
            StartCounter(true);
            dayTimer = new Timer(o => StartCounter()
                , null, TimeSpan.Zero, TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopped.");
            return Task.CompletedTask;
        }

        private int hourOfUpdate = 6; // 24H 
        private static double display = 0;
        private List<Vaccination> data = new List<Vaccination>();
        private double previousPrediction = 0;
        private double prediction = 0;
        public async void StartCounter(bool forceUpdate = false) {
            if(DateTime.Now.Hour == hourOfUpdate || forceUpdate) {
                // Update data from server
                bool update = await ServerData.Update();
                data = await ServerData.GetList();
                if(update) {
                    logger.LogInformation("Downloaded data from server.");
                }
                
                // Check if predictionDate is today or yesterday
                DateTime now = DateTime.Now;
                DateTime predictionDate;
                DateTime todayUpdateDate = DateTime.ParseExact(now.ToString("yyyy-MM-dd") + " " + hourOfUpdate + ":00", "yyyy-MM-dd H:mm", null);
                DateTime lastUpdateDate;
                if(todayUpdateDate < now) {
                    // Last update was today
                    lastUpdateDate = todayUpdateDate;
                    predictionDate = now;
                }
                else {
                    // Last update was yesterday
                    lastUpdateDate = todayUpdateDate.AddDays(-1);
                    predictionDate = now.AddDays(-1);
                }
                int secondsAfterUpdate = (int)(now - lastUpdateDate).TotalSeconds;

                // Check if data is up-to-date
                if(!ServerData.IsUpdated(data, predictionDate.AddDays(-1))) {
                    hourOfUpdate++;
                    if(hourOfUpdate == 24) {
                        hourOfUpdate = 0;
                    }
                    string warning = "Data is not up-to-date, trying again at " + hourOfUpdate.ToString("D2") + ":" + DateTime.Now.Minute.ToString("D2");
                    logger.LogWarning(warning);
                    EmailNotification.SendWarning(warning);
                    return;
                }

                // Reset hour to update
                hourOfUpdate = 6;

                // Save previous prediction
                if(prediction == 0) {
                    previousPrediction = Prediction.GetTotal(data, predictionDate.AddDays(-1));
                }
                else {
                    previousPrediction = prediction;
                }

                // Get prediction
                prediction = Prediction.GetTotal(data, predictionDate);
                double previous = TotalVaccinations.GetTotal(data, predictionDate.AddDays(-1));
                
                // Set initial value
                double startValue = display;
                dataUpdater?.Dispose();
                Randomize.ResetRandom();

                if(display == 0) {
                    display = Randomize.GetInitial(previous, prediction, secondsAfterUpdate);
                    startValue = previous;
                }

                // Run counter until next update
                dataUpdater = new Timer(o =>
                {
                    display += Randomize.GetIncrement(startValue, prediction);
                }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

                // Print information
                string dataError = Error.GetDataError(data,predictionDate.AddDays(-1));
                string error = Error.GetError(data,predictionDate.AddDays(-1));

                logger.LogInformation("Previous: " + previous);
                logger.LogInformation("Display: " + display + " after " + secondsAfterUpdate + " seconds");
                logger.LogInformation("Predicion: " + prediction);
                logger.LogInformation("Data error: " + dataError);
                logger.LogInformation("Error: " + error);
                EmailNotification.SendDailyReport(error, dataError, previous, GetDisplay(), secondsAfterUpdate, prediction, previousPrediction);
            }
        }

        public static double GetDisplay() {
            return Math.Round(display);
        }

    }
}

