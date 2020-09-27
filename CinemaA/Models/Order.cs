using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaA.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int TicketId{ get; set; }
        public string UserId { get; set; }
        public string BuyerEmail { get; set; }
        public string Name { get; set; }
        public DateTime BuyingDateTime { get; set; }
        //
        public Ticket Ticket { get; set; }
        public User User { get; set; }
    }
}
