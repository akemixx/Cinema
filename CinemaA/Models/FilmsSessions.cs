using System;
using System.Collections.Generic;

namespace CinemaA.Models
{
    public class FilmsSessions
    {
        public DateTime FilterDate { get; set; }
        public virtual ICollection<Film> Films { get; set; }

        public FilmsSessions()
        {
            Films = new List<Film>();
        }
    }
}
