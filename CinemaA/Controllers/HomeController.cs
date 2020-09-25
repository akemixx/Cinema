using System;
using System.Linq;
using CinemaA.Data;
using CinemaA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/*
 * Home Controller
 * View model: FilmsSessions
 * Functions:
 * 1) Show a list of films with their active sessions for current date (by default).
 * 2) Filter films and their sessions by the selected date. 
 */

namespace CinemaA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var viewModel = GetFilmsSessions(DateTime.Now.Date);
            return View(viewModel);
        }

        // Filter films and their sessions by date, change the list shown on a page. 
        [HttpPost]
        public IActionResult FilterByDate(DateTime filterDate)
        {
            var viewModel = GetFilmsSessions(filterDate);
            return View("Index", viewModel);
        }

        // Get films and their sessions for a specific date from the database.
        private FilmsSessions GetFilmsSessions(DateTime filterDate)
        {
            var filmsList = _context.Films
                .Include(film => film.Sessions)
                .ToList();
            foreach (var film in filmsList)
            {
                film.Sessions = film.Sessions
                    .Where(session => session.Date.Equals(filterDate) &&
                        session.Time > DateTime.Now.TimeOfDay)
                    .OrderBy(session => session.Time)
                    .ToList();
            }
            filmsList = filmsList
                .Where(film => film.Sessions.Count > 0)
                .ToList();
            return new FilmsSessions(filterDate, filmsList);
        }
    }
}
