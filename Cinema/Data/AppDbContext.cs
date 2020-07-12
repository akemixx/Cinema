using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Cinema
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) {}

        public virtual DbSet<Film> Film { get; set; }
        public virtual DbSet<Hall> Hall { get; set; }
        public virtual DbSet<Session> Session { get; set; }
        public virtual DbSet<Ticket> Ticket { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Film>(entity =>
            {
                entity.Property(e => e.Format)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('2D')");

                entity.Property(e => e.MadeIn).IsRequired();

                entity.Property(e => e.Title).IsRequired();
            });

            modelBuilder.Entity<Hall>(entity =>
            {
                entity.Property(e => e.Title).IsRequired();
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.Property(e => e.Date).HasColumnType("date");

                entity.HasOne(d => d.IdFilmNavigation)
                    .WithMany(p => p.Session)
                    .HasForeignKey(d => d.IdFilm)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Session_ToFilm");

                entity.HasOne(d => d.IdHallNavigation)
                    .WithMany(p => p.Session)
                    .HasForeignKey(d => d.IdHall)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Session_ToHall");
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('FREE')");

                entity.HasOne(d => d.IdSessionNavigation)
                    .WithMany(p => p.Ticket)
                    .HasForeignKey(d => d.IdSession)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ticket_ToSession");
            });
        }
    }
}
