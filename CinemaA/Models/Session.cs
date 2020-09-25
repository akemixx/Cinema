using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CinemaA.Models
{
    public partial class Session
    {
        public int Id { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [DataType(DataType.Time)]
        public TimeSpan Time { get; set; }
        public int IdHall { get; set; }
        public int IdFilm { get; set; }
        public double Price { get; set; }
        //
        public Film Film { get; set; }
        public Hall Hall { get; set; }
        public List<Ticket> Tickets { get; set; }
    }
}
