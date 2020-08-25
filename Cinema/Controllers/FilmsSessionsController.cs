using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cinema;
using Cinema.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Cinema.Controllers
{
    public class FilmsSessionsController : Controller
    {
        private readonly AppDbContext _context;

        public FilmsSessionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: FilmsSessions
        public async Task<IActionResult> Index()
        {
           var model = GetFilmsSessions(DateTime.Now);
           return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> FilterByDate(DateTime FilterDate)
        {
            var model = GetFilmsSessions(FilterDate);
            return View("Index", model);
        }

        private FilmsSessions GetFilmsSessions(DateTime date)
        {
            var model = new FilmsSessions
            {
                FilterDate = date
            };
            var appDbContext = _context.Film.Where(s => s.Session.Any(i => i.Date.Equals(model.FilterDate)))
                                            .Select(f => new
                                            {
                                                f,
                                                Session = f.Session.Where(e => e.Date.Equals(model.FilterDate))
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
