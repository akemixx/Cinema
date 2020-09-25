using CinemaA.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CinemaA.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Film> Films { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Film>(film =>
            {
                film.Property(f => f.Format)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('2D')");

                film.Property(f => f.MadeIn).IsRequired();
                film.Property(f => f.Title).IsRequired();
            });

            modelBuilder.Entity<Hall>(hall =>
            {
                hall.Property(h => h.Title).IsRequired();
                hall.Property(h => h.SeatsNum).IsRequired();
            });

            modelBuilder.Entity<Session>(session =>
            {
                session.Property(s => s.Date).HasColumnType("date");
                session.HasOne(s => s.Film)
                    .WithMany(f => f.Sessions)
                    .HasForeignKey(s => s.IdFilm)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Session_ToFilm");
                session.HasOne(s => s.Hall)
                    .WithMany(h => h.Sessions)
                    .HasForeignKey(s => s.IdHall)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Session_ToHall");
            });

            modelBuilder.Entity<Ticket>(ticket =>
            {
                ticket.Property(t => t.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('FREE')");
                ticket.HasOne(t => t.Session)
                    .WithMany(s => s.Tickets)
                    .HasForeignKey(t => t.IdSession)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ticket_ToSession");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}

