using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using PEF.Models;
using PEF.Settings;

namespace PEF.Services
{
    public class EmailService : IEmailService
    {
        //private readonly MailSettings _settings;
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;   
        }

        public Boolean SendEmail(EmailDto request)
        {
            //try
            //{
                Console.Write("Start Sending");
                var Mail = new MimeMessage();
                Mail.From.Add(new MailboxAddress(_config.GetSection("DisplayName").Value,_config.GetSection("Mail").Value));
                Mail.To.Add(MailboxAddress.Parse(request.to));
                Mail.Subject = request.subject;

                Mail.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = request.body };

                using var smtp = new SmtpClient();
                smtp.Connect(_config.GetSection("Host").Value, int.Parse(_config.GetSection("Port").Value), SecureSocketOptions.StartTls);
                smtp.Authenticate(_config.GetSection("Mail").Value, _config.GetSection("Password").Value);
                smtp.Send(Mail);
                smtp.Disconnect(true);
                Console.Write("INSIDE SENDING FUNC");
                return true;
            //}
            //catch(Exception ex) {
            //    Console.Write(ex.Message);
            //    return false;
            //}            
        }
    }
}
