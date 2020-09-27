using CinemaA.Mails;
using CinemaA.Models;
using CinemaA.Services;
using CinemaA.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CinemaA.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        // Create mail request, fill text with tickets data and send it to user email.
        public async Task SendEmailMessageAsync(
            string userName,
            string userEmail,
            string newTicketStatus,
            SessionTickets viewModel)
        {
            var mailRequest = new MailRequest()
            {
                ToEmail = userEmail,
                Subject = "Online cinema booking",
                Body = $"Thanks for your interest to our cinema, {userName}. Here are your tickets: "
            };
            foreach (var ticket in viewModel.SelectedTickets)
            {
                mailRequest.Body += $@"
                    <div style='background: antiquewhite;
                        font-size: 15px;
                        border: 1px solid black;
                        text-align: justify;
                        margin: 10px;
                        padding: 10px;'>
                            <p><b> Title:</b> {viewModel.Session.Film.Title} </p>
                            <p><b>Date:</b> {viewModel.Session.Date.ToShortDateString()} </p>
                            <p><b> Time:</b> {viewModel.Session.Time:hh\:mm} </p>
                            <p><b> Hall:</b> {viewModel.Session.Hall.Title} </p>
                            <p><b> Seat:</b> {ticket.IdSeat} </p>
                            <p><b> Price:</b> {viewModel.Session.Price} </p>
                            <p><b> Status:</b> {newTicketStatus} </p></br></div>";
            }
            mailRequest.Body += $@"<b>If you've booked tickets you have to pay for them at least an hour and a
                half before the session!</b>";
            try
            {
                await SendEmailAsync(mailRequest);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, MimeKit.ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();
            using (var smtp = new SmtpClient())
            {
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
            }
        }
    }
}
