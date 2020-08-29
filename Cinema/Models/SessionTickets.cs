using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema.Models
{
    public class SessionTickets
    {
        public virtual Session Session { get; set; }
        public virtual List<Ticket> SelectedTickets { get; set; }

        public SessionTickets()
        {
            SelectedTickets = new List<Ticket>();
        }

        public SessionTickets(int IdSession, AppDbContext _context) 
            : this()
        {
            Session = _context.Session.Where(item => item.Id == IdSession)
                                        .Include(s => s.Ticket)
                                        .Include(s => s.IdFilmNavigation)
                                        .Include(s => s.IdHallNavigation)
                                        .FirstOrDefault();
            Session.Ticket = Session.Ticket.OrderBy(ticket => ticket.IdSeat).ToList();
        }

        public SessionTickets(int IdSession, List<Ticket> SelectedTickets, AppDbContext _context) 
            : this(IdSession, _context)
        {
            this.SelectedTickets = SelectedTickets;
        }
    }
}
