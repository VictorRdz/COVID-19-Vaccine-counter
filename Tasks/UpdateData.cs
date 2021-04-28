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
        private static Timer dailyUpdater;

        public UpdateData(ILogger<UpdateData> logger) {
            this.logger = logger;
        }

        public void Dispose()
        {
            dailyUpdater?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {            
            Counter counterTotal = new Counter(TotalVaccinations.TOTAL);
            Counter counterPeopleFully = new Counter(TotalVaccinations.PEOPLE_FULLY);

            StartService(counterTotal, counterPeopleFully);
            dailyUpdater = new Timer(o => {
                if(DateTime.Now.Hour == hourOfUpdate) {
                    StartService(counterTotal, counterPeopleFully);
                }
            }, null, TimeSpan.Zero, TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.Dispose();
            return Task.CompletedTask;
        }

        private static List<Vaccination> data = new List<Vaccination>();
        private static int hourOfUpdate = 6; // 24H

        public async void StartService(Counter counterTotal, Counter counterPeopleFully) {
            bool upToDate = await UpdateFromServer();
            if(upToDate) {
                StartCounter(counterTotal);
                StartCounter(counterPeopleFully);
                EmailNotification.SendDailyReport(counterTotal, counterPeopleFully, GetSecondsAfterUpdate());
            }
        }

        public void StartCounter(Counter counter) {
            DateTime predictionDate = GetPredictionDate();
            // Save previous prediction
            if(counter.Prediction == 0) {
                counter.PreviousPrediction = Prediction.GetTotal(data, predictionDate.AddDays(-1), counter.Type);
            }
            else {
                counter.PreviousPrediction = counter.Prediction;
            }

            // Get prediction
            counter.Prediction = Prediction.GetTotal(data, predictionDate, counter.Type);
            counter.Previous = TotalVaccinations.GetTotal(data, predictionDate.AddDays(-1), counter.Type);
            
            // Set initial value
            double startValue = counter.Display;
            counter.Updater?.Dispose();
            Random rand = Randomize.GetRandom();

            if(counter.Display == 0) {
                counter.Display = Randomize.GetInitial(counter.Previous, counter.Prediction, GetSecondsAfterUpdate(), rand);
                startValue = counter.Previous;
            }

            // Run counter until next update
            counter.Updater = new Timer(o =>
            {
                counter.Display += Randomize.GetIncrement(startValue, counter.Prediction, rand);
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

            // Store error data
            counter.DataError = Error.GetDataError(data, predictionDate.AddDays(-1), counter.Type);
            counter.Error = Error.GetError(data, predictionDate.AddDays(-1), counter.Type);
        }

        public async Task<bool> UpdateFromServer() {
            // Update data from server
            bool update = await ServerData.Update();
            data = await ServerData.GetList();
            if(update) {
                logger.LogInformation("Downloaded data from server.");
            }
            DateTime predictionDate = GetPredictionDate();
            // Check if data is up-to-date
            if(!ServerData.IsUpdated(data, predictionDate.AddDays(-1))) {
                hourOfUpdate++;
                if(hourOfUpdate == 24) {
                    hourOfUpdate = 0;
                }
                string warning = "Data is not up-to-date, trying again at " + hourOfUpdate.ToString("D2") + ":" + DateTime.Now.Minute.ToString("D2");
                logger.LogWarning(warning);
                EmailNotification.SendWarning(warning);
                return false;
            }
            else {
                return true;
            }
        }

        public DateTime GetPredictionDate() {
            // Check if predictionDate is today or yesterday
            DateTime now = DateTime.Now;
            DateTime predictionDate;
            DateTime todayUpdateDate = DateTime.ParseExact(now.ToString("yyyy-MM-dd") + " " + hourOfUpdate + ":00", "yyyy-MM-dd H:mm", null);
            if(todayUpdateDate < now) {
                // Last update was today
                predictionDate = now;
            }
            else {
                // Last update was yesterday
                predictionDate = now.AddDays(-1);
            }
            return predictionDate;
        }

        public int GetSecondsAfterUpdate() {
            DateTime now = DateTime.Now;
            DateTime todayUpdateDate = DateTime.ParseExact(now.ToString("yyyy-MM-dd") + " " + hourOfUpdate + ":00", "yyyy-MM-dd H:mm", null);
            DateTime lastUpdateDate;
            if(todayUpdateDate < now) {
                // Last update was today
                lastUpdateDate = todayUpdateDate;
            }
            else {
                // Last update was yesterday
                lastUpdateDate = todayUpdateDate.AddDays(-1);
            }
            return (int)(now - lastUpdateDate).TotalSeconds;
        }

    }
}

