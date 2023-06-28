using System.Net.Mail;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace HW.Account.BLL.Services; 

public class EmailService : IEmailService {
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration) {
        _configuration = configuration;
    }
    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var emailMessage = new MimeMessage();
        var config = _configuration.GetSection("EmailConfiguration");

        emailMessage.From.Add(new MailboxAddress(config.GetValue<string>("FromName"),
            config.GetValue<string>("FromAddress")));
        emailMessage.To.Add(new MailboxAddress("", email));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = message
        };
             
        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(config.GetValue<string>("SmtpHost"), 465, true);
            await client.AuthenticateAsync(config.GetValue<string>("UserName"), config.GetValue<string>("Password"));
            try {
                await client.SendAsync(emailMessage);
            }
            catch (Exception e) {
                throw new NotFoundException("User with this email does not found");
            }

            await client.DisconnectAsync(true);
        }
    }
}