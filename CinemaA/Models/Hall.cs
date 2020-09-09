using System.Collections.Generic;

namespace CinemaA.Models
{
    public partial class Hall
    {
        public Hall()
        {
            Session = new List<Session>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public int SeatsNum { get; set; }

        public virtual ICollection<Session> Session { get; set; }
    }
}
