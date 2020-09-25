using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaA.Data;
using CinemaA.Models;
using CinemaA.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

/*
 * Sessions Controller
 * Model: Session
 * Functions:
 * 1) Show a list of all sessions. 
 * 2) Create a new session.
 * 3) Create a set of sessions for specific film and time for a given interval of dates.
 * 3) Edit/show details/delete sessions from the list.
 */

namespace CinemaA.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SessionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sessions
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Sessions
                .Include(session => session.Film)
                .Include(session => session.Hall)
                .OrderByDescending(session => session.Date)
                .ThenBy(session => session.Time);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Sessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions
                .Include(session => session.Film)
                .Include(session => session.Hall)
                .FirstOrDefaultAsync(session => session.Id == id);
            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        // GET: Sessions/Create
        public IActionResult Create()
        {
            ViewData["IdFilm"] = new SelectList(_context.Films
                .Where(film => film.ShownOnScreen == true),
                "Id",
                "Title");
            ViewData["IdHall"] = new SelectList(_context.Halls, "Id", "Title");
            return View();
        }

        // POST: Sessions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Time,IdHall,IdFilm,Price")] Session session)
        {
            if (ModelState.IsValid)
            {
                _context.Add(session);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdFilm"] = new SelectList(_context.Films
                .Where(film => film.ShownOnScreen == true),
                "Id",
                "Title",
                session.IdFilm);
            ViewData["IdHall"] = new SelectList(_context.Halls, "Id", "Title", session.IdHall);
            return View(session);
        }

        // GET: Sessions/CreateInterval
        public IActionResult CreateInterval()
        {
            ViewData["IdFilm"] = new SelectList(_context.Films
                .Where(film => film.ShownOnScreen == true),
                "Id",
                "Title");
            ViewData["IdHall"] = new SelectList(_context.Halls, "Id", "Title");
            return View();
        }

        // POST: Sessions/CreateInterval
        // Create a set of sessions for specific film and time for a given interval of dates.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInterval(SessionsInterval sessionsInterval)
        {
            if (ModelState.IsValid)
            {
                foreach (DateTime day in DatesRangeIterator(sessionsInterval.StartDate, sessionsInterval.EndDate))
                {
                    _context.Sessions.Add(new Session()
                    {
                        Date = day.Date,
                        IdFilm = sessionsInterval.Session.IdFilm,
                        IdHall = sessionsInterval.Session.IdHall,
                        Price = sessionsInterval.Session.Price,
                        Time = sessionsInterval.Session.Time
                    });
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdFilm"] = new SelectList(_context.Films
                .Where(film => film.ShownOnScreen == true),
                "Id",
                "Title",
                sessionsInterval.Session.IdFilm);
            ViewData["IdHall"] = new SelectList(_context.Halls, "Id", "Title", sessionsInterval.Session.IdHall);
            return View(sessionsInterval);
        }

        // Iterator for a range of dates.
        private IEnumerable<DateTime> DatesRangeIterator(DateTime startDate, DateTime endDate)
        {
            for (var day = startDate.Date; day.Date <= endDate.Date; day = day.AddDays(1))
                yield return day;
        }

        // GET: Sessions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var session = await _context.Sessions.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }
            ViewData["IdFilm"] = new SelectList(_context.Films
                .Where(film => film.ShownOnScreen == true),
                "Id",
                "Title",
                session.IdFilm);
            ViewData["IdHall"] = new SelectList(_context.Halls, "Id", "Title", session.IdHall);
            return View(session);
        }

        // POST: Sessions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Time,IdHall,IdFilm,Price")] Session session)
        {
            if (id != session.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(session);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SessionExists(session.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdFilm"] = new SelectList(_context.Films
                .Where(film => film.ShownOnScreen == true),
                "Id",
                "Title",
                session.IdFilm);
            ViewData["IdHall"] = new SelectList(_context.Halls, "Id", "Title", session.IdHall);
            return View(session);
        }

        // GET: Sessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions
                .Include(session => session.Film)
                .Include(session => session.Hall)
                .FirstOrDefaultAsync(session => session.Id == id);
            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        // POST: Sessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var session = await _context.Sessions.FindAsync(id);
            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SessionExists(int id)
        {
            return _context.Sessions.Any(session => session.Id == id);
        }
    }
}
