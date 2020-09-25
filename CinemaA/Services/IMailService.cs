using CinemaA.Mails;
using System.Threading.Tasks;

/*
 * Service used for sending emails.
 */

namespace CinemaA.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
