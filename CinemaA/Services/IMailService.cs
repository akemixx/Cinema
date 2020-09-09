using CinemaA.Mails;
using System.Threading.Tasks;

namespace CinemaA.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
