using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using NuGet.Protocol.Plugins;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;
using System.Net;

namespace A_Little_Source_Of_Hope.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger _logger;
        public EmailSender(/*IOptions<AuthMessageSenderOptions> optionsAccessor,*/ ILogger<EmailSender> logger)
        {
            //Options = optionsAccessor.Value;
            _logger = logger;
        }
        //public AuthMessageSenderOptions Options { get; }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            //if (string.IsNullOrEmpty(Options.SendGridKey))
            //{
            //    throw new NotImplementedException("Null SendGridKey");
            //}
            await Execute("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", subject, message, toEmail);
        }

        public async Task Execute(string apiKey, string subject, string message, string toEmail)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("alittlesourceofhope@outlook.com"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(toEmail));
            msg.SetClickTracking(false, false);
            var response = await client.SendEmailAsync(msg);
            _logger.LogInformation(response.IsSuccessStatusCode
                ? $"Email to {toEmail} queued successfully!" :
                $"Failure Email to {toEmail}");
        }

    }
}
