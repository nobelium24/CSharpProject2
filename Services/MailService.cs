using System.Net;
using System.Net.Mail;
using ECommerceApp.Errors;
using MimeKit;
using MailKit.Net.Smtp;

namespace ECommerceApp.Services
{
    public class SendMail
    {
        private readonly IConfiguration _configuration;
        public SendMail(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void SendEmail(string recipientMail, string recipient)
        {
            string subject = "Welcome mail";
            string body = $"Dear {recipientMail}, \nWelcome to my application. \nI hope you like it here.";
            var fromAddress = new MailboxAddress("Adewole", _configuration["Mailer:Email"] ?? throw new IsNullException());
            var toAddress = new MailboxAddress(recipient, recipientMail);
            string fromPassword = _configuration["Mailer:Password"] ?? throw new IsNullException();
            const string smtpServer = "smtp.gmail.com";
            const int smtpPort = 587;

            // var smtp = new SmtpClient
            // {
            //     Host = smtpServer,
            //     Port = smtpPort,
            //     EnableSsl = false,
            //     DeliveryMethod = SmtpDeliveryMethod.Network,
            //     UseDefaultCredentials = false,
            //     Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            // };

            // using var message = new MailMessage(fromAddress, toAddress)
            // {
            //     Subject = subject,
            //     Body = body
            // }; 
            // {
            //     smtp.Send(message);
            // }

            var email = new MimeMessage();
            email.From.Add(fromAddress);
            email.To.Add(toAddress);
            email.Subject = subject;
            email.Body = new TextPart("plain") { Text = body };

            using var client = new MailKit.Net.Smtp.SmtpClient();
            client.Connect(smtpServer, smtpPort);
            client.Authenticate(fromAddress.Address, fromPassword);
            client.Send(email);
            client.Disconnect(true);
        }
    }
}