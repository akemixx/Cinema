using CinemaA.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CinemaA.Models
{
    public class SessionTickets
    {
        public virtual Session Session { get; set; }
        public virtual List<Ticket> SelectedTickets { get; set; }

        public SessionTickets()
        {
            SelectedTickets = new List<Ticket>();
        }

        public SessionTickets(int IdSession, ApplicationDbContext _context) 
            : this()
        {
            Session = _context.Session.Where(item => item.Id == IdSession)
                                        .Include(s => s.Ticket)
                                        .Include(s => s.IdFilmNavigation)
                                        .Include(s => s.IdHallNavigation)
                                        .FirstOrDefault();
            Session.Ticket = Session.Ticket.OrderBy(ticket => ticket.IdSeat).ToList();
        }

        public SessionTickets(int IdSession, List<Ticket> SelectedTickets, ApplicationDbContext _context) 
            : this(IdSession, _context)
        {
            this.SelectedTickets = SelectedTickets;
        }
    }
}
