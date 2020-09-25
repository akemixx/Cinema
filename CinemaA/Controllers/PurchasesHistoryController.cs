using CinemaA.Data;
using CinemaA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

/*
 * PurchasesHistory Controller
 * Functions:
 * 1) Show a list of purchases for a specific authenticated user.
 */

namespace CinemaA.Controllers
{
    [Authorize]
    public class PurchasesHistoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public PurchasesHistoryController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            string userEmail = _userManager.GetUserAsync(User).Result.Email;
            return View(_context.Tickets
                .Include(ticket => ticket.Session)
                .ThenInclude(session => session.Film)
                .Include(session => session.Session.Hall)
                .Where(ticket => ticket.BuyerEmail == userEmail)
                .OrderBy(ticket => ticket.BuyingDateTime)
                .ToList()
                );
        }
    }
}
