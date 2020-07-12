using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cinema
{
    public partial class Session
    {
        public Session()
        {
            Ticket = new List<Ticket>();
        }

        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan Time { get; set; }

        public int IdHall { get; set; }
        public int IdFilm { get; set; }
        public double Price { get; set; }

        public virtual Film IdFilmNavigation { get; set; }
        public virtual Hall IdHallNavigation { get; set; }
        public virtual ICollection<Ticket> Ticket { get; set; }
    }
}
