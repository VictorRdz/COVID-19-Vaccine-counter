using System;
using System.Net;
using System.Net.Mail;

namespace covid19_backend.Tasks
{
    public class EmailNotification {

        public static void SendDailyReport(double error, double actualValue, double displayValue, double prediction) 
        {
            string subject = "DAILY REPORT  - COVID-19: " + DateTime.Now.ToString("dd-MM-yyyy - h:mm tt");
            string color = "#32CD32"; // Lime green
            string auxError = String.Format("{0:0.00}", error) + "%"; 
            if(Math.Abs(error) >= 5) {
                color = "#A71010";
            }
            string body = $"<b>Data is up-to-date</b><br><br><b>Initial value: </b><span>{ actualValue }</span><br><b>Display value: </b><span>{ displayValue }</span><br><b>Prediction: </b><span>{ prediction }</span><br><b>Error: </b><span style='color: { color }'>{ auxError }</span>";

            EmailNotification.SendEmail(subject, body, true);
        }

        public static void SendWarning(string message) 
        {
            string subject = "WARNING!!  - COVID-19: " + DateTime.Now.ToString("dd-MM-yyyy - h:mm tt");
            string body = $"<b style='color:#A71010'>{message}</b>";

            EmailNotification.SendEmail(subject, body, true);
        }


        public static void SendEmail(string subject, string body, bool isHtml) {
            var fromAddress = new MailAddress(Accounts.getEmailAddress(), "COVID-19 Counter");
            var toAddress = new MailAddress(Accounts.getEmailAddress(), "Victor Gonzalez");
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