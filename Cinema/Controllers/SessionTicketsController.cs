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
            if(Request.Form.FirstOrDefault(x => x.Key == "BuyTickets").Key != null) // buy tickets
            {
                return ProcessTickets(IdSession, BookedSeats, "BUSY");
            }
            // book tickets
            return ProcessTickets(IdSession, BookedSeats, "BOOKED");
        }

        //[HttpPost]
        public IActionResult ProcessTickets(int IdSession, List<int> BookedSeats, string NewStatus)
        {
            foreach (int IdSeat in BookedSeats)
            {
                var ChangedTicket = _context.Ticket.Where(item => item.IdSession == IdSession && item.IdSeat == IdSeat)
                                               .FirstOrDefault();
                _context.Ticket.Attach(ChangedTicket); // State = Unchanged
                ChangedTicket.Status = NewStatus; // State = Modified, and only the Status property is dirty.
            }
            _context.SaveChanges();
            //SessionTickets model = new SessionTickets(IdSession, BookedTickets, _context);
            //return View("BuyingTicket", model);
            return Index(IdSession);
        }
    }
}