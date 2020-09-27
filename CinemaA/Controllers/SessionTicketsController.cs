using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaA.Mails;
using CinemaA.Services;
using CinemaA.Data;
using Microsoft.AspNetCore.Mvc;
using CinemaA.Models;
using Microsoft.AspNetCore.Identity;

/*
 * SessionTickets Controller
 * View model: SessionTickets
 * Functions:
 * 1) Show a scheme of seats in the hall.
 * 2) Buy/book tickets of a specific session.
 * 3) Send tickets by email.
 * 4) Renew bonuses count for authenticated users (bonuses are awarded for ticket purchases).
 */

namespace CinemaA.Controllers
{
    public class SessionTicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMailService _mailService;
        private readonly UserManager<User> _userManager;

        public SessionTicketsController(
            ApplicationDbContext context, 
            IMailService mailService, 
            UserManager<User> userManager)
        {
            _context = context;
            _mailService = mailService;
            _userManager = userManager;
        }

        // GET: SessionTickets
        public IActionResult Index(int sessionId)
        {
            var viewModel = new SessionTickets(sessionId, _context);
            return View("SessionTickets", viewModel);
        }

        // Get ids of tickets selected by user and show a ticket cart.
        [HttpPost]
        public IActionResult SelectSeats(int sessionId, List<int> selectedTicketsIds)
        {
            SessionTickets viewModel = CreateViewModel(sessionId, selectedTicketsIds);
            return View("TicketCart", viewModel);
        }

        // Get ticket cart with selected tickets, change their statuses (bought/booked).
        // Renew bonuses count and send email with tickets.
        [HttpPost]
        public async Task<IActionResult> ChooseFormAction(
            int sessionId, 
            List<int> selectedTicketsIds, 
            string totalPrice, 
            string userName, 
            string userEmail,
            string formAction)
        {
            if (ModelState.IsValid)
            {
                SessionTickets viewModel = CreateViewModel(sessionId, selectedTicketsIds);
                User user = new User();
                user.Email = userEmail;
                user.RealName = userName;
                string newTicketStatus;
                if (formAction == "Buy tickets")
                {
                    newTicketStatus = "BOUGHT";
                }
                else
                {
                    newTicketStatus = "BOOKED";
                }
                if (User.Identity.IsAuthenticated)
                {
                    user = await _userManager.GetUserAsync(User);
                    if(formAction == "Buy tickets")
                    {
                        double newTotalPrice = Convert.ToDouble(totalPrice.Replace('.', ','));
                        user.Bonuses += Convert.ToDouble((newTotalPrice * 0.1).ToString("N2"));
                        await _userManager.UpdateAsync(user);
                    }
                }
                ProcessTickets(viewModel, newTicketStatus, user);
                try
                {
                    await _mailService.SendEmailMessageAsync(user.RealName, user.Email, newTicketStatus, viewModel);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
                return View("Succeeded");
            }
            return View("Error");
        }

        // Renew user bonuses count using data from AJAX request.
        [HttpPost]
        public async Task RenewBonusesCountAjax([FromBody] double userBonuses)
        {
            var user = await _userManager.GetUserAsync(User);
            user.Bonuses = userBonuses;
            await _userManager.UpdateAsync(user);
        }

        // Create SessionTickets view model using id of session and list of tickets ids.
        private SessionTickets CreateViewModel(int sessionId, List<int> selectedTicketsIds)
        {
            var viewModel = new SessionTickets(sessionId, _context);
            Ticket changedTicket;
            foreach (int id in selectedTicketsIds)
            {
                changedTicket = viewModel.Session.Tickets
                    .Where(ticket => ticket.Id == id)
                    .FirstOrDefault();
                viewModel.SelectedTickets.Add(changedTicket);
            }
            return viewModel;
        }

        // Change status of the ticket in DB to booked or bought, create a new order.
        private void ProcessTickets(SessionTickets viewModel, string newTicketStatus, User user)
        {
            int SessionId = viewModel.Session.Id;
            Ticket ChangedTicket;
            var newOrder = new Order();
            if (_context.Users.Find(user.Id) != null)
            {
                newOrder.UserId = user.Id;
            }
            else
            {
                newOrder.BuyerEmail = user.Email;
                newOrder.Name = user.RealName;
            }
            newOrder.BuyingDateTime = DateTime.Now;
            foreach (var ticket in viewModel.SelectedTickets)
            {
                ChangedTicket = _context.Tickets
                    .Where(_ticket => _ticket.Id == ticket.Id)
                    .FirstOrDefault();
                _context.Tickets.Attach(ChangedTicket); 
                ChangedTicket.Status = newTicketStatus;
                newOrder.TicketId = ticket.Id;
                _context.Add(newOrder);
                _context.SaveChanges();
            }
        }
    }
}