using CinemaA.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

/*
 * Used in SessionsTickets controller
 */

namespace CinemaA.Models
{
    public class SessionTickets
    {
        // Session selected by user.
        public Session Session { get; set; }
        // Tickets selected by user.
        public List<Ticket> SelectedTickets { get; set; }

        public SessionTickets(int IdSession, ApplicationDbContext _context) 
        {
            Session = _context.Sessions
                .Where(session => session.Id == IdSession)
                .Include(session => session.Tickets)
                .Include(session => session.Film)
                .Include(session => session.Hall)
                .FirstOrDefault();
            Session.Tickets = Session.Tickets
                .OrderBy(ticket => ticket.IdSeat)
                .ToList();
            SelectedTickets = new List<Ticket>();
        }

        public SessionTickets(int IdSession, List<Ticket> SelectedTickets, ApplicationDbContext _context) 
            : this(IdSession, _context)
        {
            this.SelectedTickets = SelectedTickets;
        }
    }
}
