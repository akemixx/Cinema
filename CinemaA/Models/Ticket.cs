using System;

namespace CinemaA.Models
{
    public partial class Ticket
    {
        public int Id { get; set; }
        public int IdSession { get; set; }
        public int IdSeat { get; set; }
        public string Status { get; set; }
        //public string BuyerEmail { get; set; }
        //public DateTime BuyingDateTime { get; set; }
        //
        public Session Session { get; set; }
        public Order Order { get; set; }
    }
}
