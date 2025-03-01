using Core_Layer;
using Core_Layer.Inetrfaces.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendEmail(Email email)
        {
            // cerate new client by pass host and port
            var client = new SmtpClient(_config["EmailConfiguration:Host"], int.Parse(_config["EmailConfiguration:Port"]));
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(_config["EmailConfiguration:Email"], _config["EmailConfiguration:Password"]);
            client.Send(_config["EmailConfiguration:Email"], email.To, email.Subject, email.Body);
        }
    }
}
