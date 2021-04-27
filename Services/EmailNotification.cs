using System;
using System.Net;
using System.Net.Mail;

namespace covid19_backend.Services
{
    public class EmailNotification {

        public static void SendDailyReport(string error, string dataError, double previous, double display, int secondsAfterUpdate, double prediction, double previousPrediction) 
        {
            string subject = "DAILY REPORT  - COVID-19: " + DateTime.Now.ToString("dd-MM-yyyy - h:mm tt");
            string color = "#32CD32"; // Lime green
            double doubleDataError = double.Parse(dataError.Replace("%",""));
            string auxError = String.Format("{0:0.00}", error) + "%"; 
            if(Math.Abs(doubleDataError) >= 5) {
                color = "#A71010";
            }
            string body = $"<b>Data is up-to-date</b><br><br><b>Previous prediction: </b><span>{ previousPrediction }</span><br><b>Previous: </b><span>{ previous }</span><br><b>Display: </b><span>{ display } after { secondsAfterUpdate } seconds</span><br><b>Prediction: </b><span>{ prediction }</span><br><br><b>Prediction error: </b><span>{ error }</span><br><b>Data error: </b><span style='color: { color }'>{ dataError }</span>";

            EmailNotification.SendEmail(subject, body, true);
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