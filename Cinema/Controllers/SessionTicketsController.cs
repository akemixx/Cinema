using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cinema.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Controllers
{
    public class SessionTicketsController : Controller
    {
        private readonly AppDbContext _context;

        public SessionTicketsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: SessionTickets
        public IActionResult Index(int IdSession)
        {
            SessionTickets model = new SessionTickets(IdSession, _context);
            return View("SessionTickets", model);
        }

        [HttpPost]
        public IActionResult SelectSeats(int IdSession, List<int> BookedSeats)
        {
            List<Ticket> BookedTickets = new List<Ticket>();
            foreach (int IdSeat in BookedSeats)
            {
                var ChangedTicket = _context.Ticket.Where(item => item.IdSession == IdSession && item.IdSeat == IdSeat)
                                               .FirstOrDefault();
                BookedTickets.Add(ChangedTicket);
            }
            SessionTickets model = new SessionTickets(IdSession, BookedTickets, _context);
            return View("BuyingTicket", model);
        }

        [HttpPost]
        public IActionResult ChooseFormAction(int IdSession, List<int> BookedSeats)
        {
            if(Request.Form.FirstOrDefault(x => x.Value == "Delete").Key != null) // delete ticket
            {
                var DeleteSeatNum = Convert.ToInt32(Request.Form.FirstOrDefault(x => x.Value == "Delete").Key);
                BookedSeats.RemoveAt(DeleteSeatNum);
                return SelectSeats(IdSession, BookedSeats);
            }
            if(Request.Form.FirstOrDefault(x => x.Key == "BuyTickets").Key != null) // buy tickets
            {
                return BuyTickets(IdSession, BookedSeats, "BUSY");
            }
            if (Request.Form.FirstOrDefault(x => x.Key == "BookTickets").Key != null) // book tickets
            {
                return BuyTickets(IdSession, BookedSeats, "BOOKED");
            }
            return SelectSeats(IdSession, BookedSeats);
        }
        //public IActionResult DeleteSelectedSeat(int IdSession, List<int> BookedSeats)
        //{
        //    var DeleteSeatNum = Convert.ToInt32(Request.Form.FirstOrDefault(x => x.Value == "Delete").Key);
        //    BookedSeats.RemoveAt(DeleteSeatNum);

        //}

        //[HttpPost]
        public IActionResult BuyTickets(int IdSession, List<int> BookedSeats, string NewStatus)
        {
            //List<Ticket> BookedTickets = new List<Ticket>();
            foreach (int IdSeat in BookedSeats)
            {
                var ChangedTicket = _context.Ticket.Where(item => item.IdSession == IdSession && item.IdSeat == IdSeat)
                                               .FirstOrDefault();
                _context.Ticket.Attach(ChangedTicket); // State = Unchanged
                ChangedTicket.Status = NewStatus; // State = Modified, and only the Status property is dirty.
                //BookedTickets.Add(ChangedTicket);
            }
            _context.SaveChanges();
            //SessionTickets model = new SessionTickets(IdSession, BookedTickets, _context);
            //return View("BuyingTicket", model);
            return Index(IdSession);
        }
    }
}