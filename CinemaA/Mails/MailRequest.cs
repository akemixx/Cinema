using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

/* 
 * Used for sending tickets bought/booked by a user using email.
 */
namespace CinemaA.Mails
{
    public class MailRequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }
}
