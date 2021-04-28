using System;
using System.Net;
using System.Net.Mail;
using covid19_backend.Models;

namespace covid19_backend.Services
{
    public class EmailNotification {

        public static void SendDailyReport(Counter counterTotal, Counter counterPeopleFully, int secondsAfterUpdate) 
        {
            string subject = "DAILY REPORT  - COVID-19: " + DateTime.Now.ToString("dd-MM-yyyy - h:mm tt");

            // Formatting for total counter
            string colorT = "#32CD32"; // Lime green
            double doubleDataErrorT = double.Parse(counterTotal.DataError.Replace("%",""));
            string auxErrorT = String.Format("{0:0.00}", counterTotal.Error) + "%"; 
            if(Math.Abs(doubleDataErrorT) >= 5) {
                colorT = "#A71010";
            }

            // Formatting for people fully counter
            string colorP = "#32CD32"; // Lime green
            double doubleDataErrorP = double.Parse(counterPeopleFully.DataError.Replace("%",""));
            string auxErrorP = String.Format("{0:0.00}", counterPeopleFully.Error) + "%"; 
            if(Math.Abs(doubleDataErrorP) >= 5) {
                colorP = "#A71010";
            }

            string body1 = "<h1 style='color:#32CD32'>Data is up-to-date</h1><hr>";
            string body2 = $"<h2>Total Vaccinations</h2><b>Previous prediction: </b><span>{ counterTotal.PreviousPrediction }</span><br><b>Previous: </b><span>{ counterTotal.Previous }</span><br><b>Display: </b><span>{ counterTotal.GetDisplay() } after { secondsAfterUpdate } seconds</span><br><b>Prediction: </b><span>{ counterTotal.Prediction }</span><br><br><b>Prediction error: </b><span>{ counterTotal.Error }</span><br><b>Data error: </b><span style='color: { colorT }'>{ counterTotal.DataError }</span><hr>";
            string body3 = $"<h2>People Fully Vaccinated</h2><b>Previous prediction: </b><span>{ counterPeopleFully.PreviousPrediction }</span><br><b>Previous: </b><span>{ counterPeopleFully.Previous }</span><br><b>Display: </b><span>{ counterPeopleFully.GetDisplay() } after { secondsAfterUpdate } seconds</span><br><b>Prediction: </b><span>{ counterPeopleFully.Prediction }</span><br><br><b>Prediction error: </b><span>{ counterPeopleFully.Error }</span><br><b>Data error: </b><span style='color: { colorP }'>{ counterPeopleFully.DataError }</span><hr>";
            EmailNotification.SendEmail(subject, body1 + body2 + body3, true);
        }

        public static void SendWarning(string message) 
        {
            string subject = "WARNING!!  - COVID-19: " + DateTime.Now.ToString("dd-MM-yyyy - h:mm tt");
            string body = $"<b style='color:#A71010'>{message}</b>";

            EmailNotification.SendEmail(subject, body, true);
        }


        public static void SendEmail(string subject, string body, bool isHtml) {
            var fromAddress = new MailAddress(Accounts.getFromEmailAddress(), "COVID-19 App");
            var toAddress = new MailAddress(Accounts.getToEmailAddress(), "Victor Gonzalez");
            string fromPassword = Accounts.getPassword();

            var smtp = new SmtpClient
            {
                Host = "smtp-mail.outlook.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                message.IsBodyHtml = isHtml;
                smtp.Send(message);
            }
        }

    }
}