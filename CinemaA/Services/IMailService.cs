using CinemaA.Mails;
using CinemaA.Models;
using System.Threading.Tasks;

/*
 * Service used for sending emails.
 */

namespace CinemaA.Services
{
    public interface IMailService
    {
        Task SendEmailMessageAsync(string userName,
            string userEmail,
            string newTicketStatus,
            SessionTickets viewModel);
    }
}
