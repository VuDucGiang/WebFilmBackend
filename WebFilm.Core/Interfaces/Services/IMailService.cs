using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Mail;

namespace WebFilm.Core.Interfaces.Services
{
    public interface IMailService
    {
        Task<bool> SendAsync(MailData mailData, CancellationToken ct);

        string GetEmailTemplate(string emailTemplate, MailTemplate emailTemplateModel);
    }
}
