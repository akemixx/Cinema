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
                string newTicketStatus;
                if (User.Identity.IsAuthenticated)
                {
                    var user = await _userManager.GetUserAsync(User);
                    userName = user.RealName;
                    userEmail = user.Email;
                    if(formAction == "Buy tickets")
                    {
                        double newTotalPrice = Convert.ToDouble(totalPrice.Replace('.', ','));
                        user.Bonuses += Convert.ToDouble((newTotalPrice * 0.1).ToString("N2"));
                        await _userManager.UpdateAsync(user);
                    }
                }
                if (formAction == "Buy tickets")
                {
                    if (User.Identity.IsAuthenticated)
                    {

                    }
                    ProcessTickets(viewModel, "BUSY", userEmail);
                    newTicketStatus = "Bought";
                }
                else
                {
                    ProcessTickets(viewModel, "BOOKED", userEmail);
                    newTicketStatus = "Booked";
                }
                try
                {
                    await SendEmailMessageAsync(userName, userEmail, newTicketStatus, viewModel);
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

        // Create mail request, fill text with tickets data and send it to user email.
        private async Task SendEmailMessageAsync(
            string userName, 
            string userEmail, 
            string newTicketStatus, 
            SessionTickets viewModel)
        {
            var mailRequest = new MailRequest()
            {
                ToEmail = userEmail,
                Subject = "Online cinema booking",
                Body = $"Thanks for your interest to our cinema, {userName}. Here are your tickets: "
            };
            foreach (var ticket in viewModel.SelectedTickets)
            {
                mailRequest.Body += $@"
                    <div style='background: antiquewhite;
                        font-size: 15px;
                        border: 1px solid black;
                        text-align: justify;
                        margin: 10px;
                        padding: 10px;'>
                            <p><b> Title:</b> {viewModel.Session.Film.Title} </p>
                            <p><b>Date:</b> {viewModel.Session.Date.ToShortDateString()} </p>
                            <p><b> Time:</b> {viewModel.Session.Time:hh\:mm} </p>
                            <p><b> Hall:</b> {viewModel.Session.Hall.Title} </p>
                            <p><b> Seat:</b> {ticket.IdSeat} </p>
                            <p><b> Price:</b> {viewModel.Session.Price} </p>
                            <p><b> Status:</b> {newTicketStatus} </p></br></div>";
            }
            mailRequest.Body += $@"<b>If you've booked tickets you have to pay for them at least an hour and a
                half before the session!</b>";
            try
            {
                await _mailService.SendEmailAsync(mailRequest);
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

        // Change status of the ticket in DB to booked or bought, mark the time of purchase and the buyer.
        private void ProcessTickets(SessionTickets viewModel, string newTicketStatus, string userEmail)
        {
            int SessionId = viewModel.Session.Id;
            Ticket ChangedTicket;
            foreach (var ticket in viewModel.SelectedTickets)
            {
                ChangedTicket = _context.Tickets
                    .Where(_ticket => _ticket.Id == ticket.Id)
                    .FirstOrDefault();
                _context.Tickets.Attach(ChangedTicket); 
                ChangedTicket.Status = newTicketStatus;
                ChangedTicket.BuyerEmail = userEmail;
                ChangedTicket.BuyingDateTime = DateTime.Now;
            }
            _context.SaveChanges();
        }
    }
}