using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace PizzashopMVCProject.Utilty;

public class EmailSender : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // var emailToSend = new MimeMessage();
        // emailToSend.From.Add(MailboxAddress.Parse("maheshnanera3204@gmail.com"));
        // emailToSend.To.Add(MailboxAddress.Parse(email));
        // emailToSend.Subject = subject;
        // emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html){ Text = htmlMessage};

        // //send Email
        // using(var emailClient = new SmtpClient()){

        // }

        var message = new MailMessage();
        message.To.Add(email);
        message.Subject = subject;
        message.Body = htmlMessage;
        message.IsBodyHtml = true;
        message.From = new MailAddress("maheshnanera1619@gmail.com");

        using var smtp = new SmtpClient("mail.etatvasoft.com", 587);
        // smtp.EnableSsl = _options.EnableSsl;
        smtp.Credentials = new NetworkCredential("test.dotnet@etatvasoft.com", "P}N^{z-]7Ilp");

        await smtp.SendMailAsync(message);
        // return Task.CompletedTask;
    }

}
