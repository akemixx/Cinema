using System.Collections.Generic;

namespace CinemaA.Models
{
    public partial class Film
    {
        public Film()
        {
            Session = new List<Session>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Annotation { get; set; }
        public string Genre { get; set; }
        public string MadeIn { get; set; }
        public string Format { get; set; }

        public virtual ICollection<Session> Session { get; set; }
    }
}
