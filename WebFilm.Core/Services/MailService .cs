﻿using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;
using RazorEngineCore;
using System.Net.Http;
using System.Text;
using WebFilm.Core.Enitites.Mail;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Interfaces.Services;
using WebFilm.Core.Resources;

namespace WebFilm.Core.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _settings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MailService(IOptions<MailSettings> settings, IHttpContextAccessor httpContextAccessor)
        {
            _settings = settings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> SendAsync(MailData mailData, CancellationToken ct = default)
        {
            try
            {
                // Initialize a new instance of the MimeKit.MimeMessage class
                var mail = new MimeMessage();

                #region Sender / Receiver
                // Sender
                mail.From.Add(new MailboxAddress(_settings.DisplayName, mailData.From ?? _settings.From));
                mail.Sender = new MailboxAddress(mailData.DisplayName ?? _settings.DisplayName, mailData.From ?? _settings.From);

                

                // Receiver
                foreach (string mailAddress in mailData.To)
                    mail.To.Add(MailboxAddress.Parse(mailAddress));

                // Set Reply to if specified in mail data
                if (!string.IsNullOrEmpty(mailData.ReplyTo))
                    mail.ReplyTo.Add(new MailboxAddress(mailData.ReplyToName, mailData.ReplyTo));

                // BCC
                // Check if a BCC was supplied in the request
                if (mailData.Bcc != null)
                {
                    // Get only addresses where value is not null or with whitespace. x = value of address
                    foreach (string mailAddress in mailData.Bcc.Where(x => !string.IsNullOrWhiteSpace(x)))
                        mail.Bcc.Add(MailboxAddress.Parse(mailAddress.Trim()));
                }

                // CC
                // Check if a CC address was supplied in the request
                if (mailData.Cc != null)
                {
                    foreach (string mailAddress in mailData.Cc.Where(x => !string.IsNullOrWhiteSpace(x)))
                        mail.Cc.Add(MailboxAddress.Parse(mailAddress.Trim()));
                }
                #endregion

                #region Content

                // Add Content to Mime Message
                var body = new BodyBuilder();
                mail.Subject = mailData.Subject;
                body.HtmlBody = mailData.Body;
                mail.Body = body.ToMessageBody();

                #endregion

                #region Send Mail

                using var smtp = new SmtpClient();

                

                if (_settings.UseSSL){}

                await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls, ct);
                await smtp.AuthenticateAsync(_settings.UserName, _settings.Password, ct);
                await smtp.SendAsync(mail, ct);
                await smtp.DisconnectAsync(true, ct);

                

                #endregion

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetEmailTemplate(string emailTemplate, MailTemplate emailTemplateModel)
        {
            string mailTemplate = LoadTemplate(emailTemplate);

            IRazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate modifiedMailTemplate = razorEngine.Compile(mailTemplate);

            return modifiedMailTemplate.Run(emailTemplateModel);
        }

        public string LoadTemplate(string emailTemplate)
        {
            HttpContext httpContext = _httpContextAccessor.HttpContext;
            if (IsLocalRequest(httpContext))
            {
                // Xử lý cho yêu cầu từ localhost
                string templateDir = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
                string templatePath = templateDir + $"\\WebFilm.Core\\Enitites\\Mail\\{emailTemplate}.html";

                using FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using StreamReader streamReader = new StreamReader(fileStream, Encoding.Default);

                string mailTemplate = streamReader.ReadToEnd();
                streamReader.Close();

                return mailTemplate;
            } else
            {
                string templatePath = Path.Combine("WebFilm.Core", "Enitites", "Mail", $"{emailTemplate}.html");

                using (FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader streamReader = new StreamReader(fileStream, Encoding.Default))
                    {
                        string mailTemplate = streamReader.ReadToEnd();
                        return mailTemplate;
                    }
                }
            }
            //string templateDir = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            //string templatePath = templateDir + $"\\WebFilm.Core\\Enitites\\Mail\\{emailTemplate}.html";

            //using FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            //using StreamReader streamReader = new StreamReader(fileStream, Encoding.Default);

            //string mailTemplate = streamReader.ReadToEnd();
            //streamReader.Close();

            //return mailTemplate;
        }

        private bool IsLocalRequest(HttpContext httpContext)
        {

            string ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            bool isLocal = ipAddress == "::1" || ipAddress == "127.0.0.1" || ipAddress == "::ffff:127.0.0.1";

            return isLocal;
        }
    }
}
