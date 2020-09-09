namespace CinemaA.Models
{
    public partial class Ticket
    {
        public int Id { get; set; }
        public int IdSession { get; set; }
        public int IdSeat { get; set; }
        public string Status { get; set; }

        public virtual Session IdSessionNavigation { get; set; }
    }
}
