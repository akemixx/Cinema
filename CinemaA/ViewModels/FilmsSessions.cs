using System;
using System.Collections.Generic;

/*
 * Used in Home controller views.
 */

namespace CinemaA.Models
{
    public class FilmsSessions
    {
        // Date by which sessions of films are filtered.
        public DateTime FilterDate { get; set; }
        // Films which are a result of filtering by filter date.
        public List<Film> Films { get; set; }

        public FilmsSessions(DateTime filterDate, List<Film> films)
        {
            FilterDate = filterDate;
            Films = films;
        }
    }
}
