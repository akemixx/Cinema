﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CinemaA.Mails;
using CinemaA.Services;
using CinemaA.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CinemaA.Models;
using Microsoft.AspNetCore.Identity;

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
        public IActionResult Index(int IdSession)
        {
            SessionTickets model = new SessionTickets(IdSession, _context);
            return View("SessionTickets", model);
        }

        // selecting seats
        [HttpPost]
        public IActionResult SelectSeats(int IdSession, List<int> SelectedSeats)
        {
            SessionTickets model = CreateModel(IdSession, SelectedSeats);
            return View("TicketCart", model);
        }

        // booking/buying tickets from ticket cart and sending email message
        [HttpPost]
        public async Task<IActionResult> ChooseFormAction(int IdSession, List<int> SelectedSeats, string totalPrice, string Name, string Email)
        {
            SessionTickets model = CreateModel(IdSession, SelectedSeats);
            if (ModelState.IsValid)
            {
                string status;
                if (Request.Form.FirstOrDefault(x => x.Key == "BuyTickets").Key != null) // buy tickets
                {
                    ProcessTickets(IdSession, SelectedSeats, "BUSY");
                    status = "Bought";
                    if (User.Identity.IsAuthenticated)
                    {
                        var user = await _userManager.GetUserAsync(User);
                        Name = user.NameOfUser;
                        Email = user.Email;
                        double newTotalPrice = Convert.ToDouble(totalPrice.Replace('.', ','));
                        user.Bonuses += Convert.ToDouble((newTotalPrice * 0.1).ToString("N2"));
                        await _userManager.UpdateAsync(user);
                    }
                }
                else  // book tickets
                {
                    ProcessTickets(IdSession, SelectedSeats, "BOOKED");
                    status = "Booked";
                }
                try
                {
                    await SendEmailMessageAsync(Name, Email, status, model);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
                return View("Succeeded");
            }
            return View("Error");
        }

        // renew personal bonuses count
        [HttpPost]
        public async Task RenewBonusesCount([FromBody] double bonuses)
        {
            var user = await _userManager.GetUserAsync(User);
            user.Bonuses = bonuses;
            await _userManager.UpdateAsync(user);
        }

        // creating mail and sending it to user
        private async Task SendEmailMessageAsync(string Name, string Email, string status, SessionTickets model)
        {
            var request = new MailRequest()
            {
                ToEmail = Email,
                Subject = "Online cinema booking",
                Body = $"Thanks for your interest to our cinema, {Name}. Here are your tickets: "
            };
            foreach (var ticket in model.SelectedTickets)
            {
                request.Body += $@"<div style='background: antiquewhite;
                                    font-size: 15px;
                                    border: 1px solid black;
                                    text-align: justify;
                                    margin: 10px;
                                    padding: 10px;'>
                                <p><b> Title:</b> {model.Session.IdFilmNavigation.Title} </p>
                                <p><b>Date:</b> {model.Session.Date.ToShortDateString()} </p>
                                <p><b> Time:</b> {model.Session.Time:hh\:mm} </p>
                                <p><b> Hall:</b> {model.Session.IdHallNavigation.Title} </p>
                                <p><b> Seat:</b> {ticket.IdSeat} </p>
                                <p><b> Price:</b> {model.Session.Price} </p>
                                <p><b> Status:</b> {status} </p></br></div>";
            }
            request.Body += "<b>If you've booked tickets you have to pay for them at least an hour and a half before the session!</b>";
            try
            {
                await _mailService.SendEmailAsync(request);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // create SessionTickets model from session id and list of selected seats
        private SessionTickets CreateModel(int IdSession, List<int> SelectedSeats)
        {
            List<Ticket> SelectedTickets = new List<Ticket>();
            foreach (int IdSeat in SelectedSeats)
            {
                var ChangedTicket = _context.Ticket.Where(item => item.IdSession == IdSession && item.IdSeat == IdSeat)
                                               .FirstOrDefault();
                SelectedTickets.Add(ChangedTicket);
            }
            return new SessionTickets(IdSession, SelectedTickets, _context);
        }

        // change status of ticket in the db to bought or booked
        private void ProcessTickets(int IdSession, List<int> SelectedSeats, string NewStatus)
        {
            foreach (int IdSeat in SelectedSeats)
            {
                var ChangedTicket = _context.Ticket.Where(item => item.IdSession == IdSession && item.IdSeat == IdSeat)
                                               .FirstOrDefault();
                _context.Ticket.Attach(ChangedTicket); // State = Unchanged
                ChangedTicket.Status = NewStatus; // State = Modified, and only the Status property is dirty.
            }
            _context.SaveChanges();
        }
    }
}