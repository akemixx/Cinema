using System.Collections.Generic;

namespace CinemaA.Models
{
    public partial class Hall
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int SeatsNum { get; set; }
        //
        public List<Session> Sessions { get; set; }
    }
}
