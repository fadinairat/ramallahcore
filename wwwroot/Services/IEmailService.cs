﻿using PEF.Models;

namespace PEF.Services
{
    //private readonly MailSettings _mailSettings;
    //public IEmailService (IOptions<MailSettings> mailSettings)
    //{
    //    _mailSettings = mailSettings.Value;
    //}
    public interface IEmailService
    {        
        Boolean SendEmail(EmailDto request);
    }
}
