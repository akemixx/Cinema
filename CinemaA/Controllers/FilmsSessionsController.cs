using System;
using System.Collections.Generic;
using System.Linq;
using CinemaA.Data;
using CinemaA.Models;
using Microsoft.AspNetCore.Mvc;

namespace CinemaA.Controllers
{
    public class FilmsSessionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FilmsSessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FilmsSessions
        public IActionResult Index()
        {
           var model = GetFilmsSessions(DateTime.Now);
           return View(model);
        }

        [HttpPost]
        public IActionResult FilterByDate(DateTime FilterDate)
        {
            var model = GetFilmsSessions(FilterDate);
            return View("Index", model);
        }

        // get from db sessions on concrete date
        private FilmsSessions GetFilmsSessions(DateTime date)
        {
            var model = new FilmsSessions
            {
                FilterDate = date
            };
            var appDbContext = _context.Film.Where(s => s.Session.Any(i => i.Date.Equals(model.FilterDate) && i.Time > DateTime.Now.TimeOfDay))
                                            .Select(f => new
                                            {
                                                f,
                                                Session = f.Session.Where(e => e.Date.Equals(model.FilterDate) && e.Time > DateTime.Now.TimeOfDay)
                                            });
            var list = appDbContext.ToList();
            var films = new List<Film>();
            foreach (var el in list)
            {
                el.f.Session = el.Session.OrderBy(s => s.Time).ToList();
                films.Add(el.f);
            }
            model.Films = films;
            return model;
        }

        // get ajax request for filtering sessions by date
        public IActionResult FilterByDateAjax([FromBody] DateTime FilterDate)
        {
            if (ModelState.IsValid)
            {
                var appDbContext = _context.Film.Where(s => s.Session.Any(i => i.Date.Equals(FilterDate)))
                                            .Select(f => new
                                            {
                                                f,
                                                Session = f.Session.Where(e => e.Date.Equals(FilterDate))
                                            });
                var list = appDbContext.ToList();
                var films = new List<Film>();
                foreach (var el in list)
                {
                    el.f.Session = el.Session.OrderBy(s => s.Time).ToList();
                    films.Add(el.f);
                }
                return new JsonResult(new { films = Json(films) });
            }
            return BadRequest(ModelState["FilterDate"].Errors[0].ErrorMessage);
        }
    }
}
