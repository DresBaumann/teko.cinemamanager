using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TopMovie.CinemaManager.Core.Features.Cinemas.Models;
using TopMovie.CinemaManager.Core.Features.Memberships.Models;
using TopMovie.CinemaManager.Core.Features.Reservations.Models;
using TopMovie.CinemaManager.Core.Features.Screenings.Models;

namespace TopMovie.CinemaManager.Framework.Data
{
	public class CinemaManagerDbContext : IdentityDbContext<Member>
	{
		public CinemaManagerDbContext(DbContextOptions<CinemaManagerDbContext> options)
			: base(options)
		{
		}
		
		public DbSet<Cinema> Cinemas { get; set; }
		public DbSet<CinemaHall> CinemaHalls { get; set; }
		public DbSet<SeatIdentifier> SeatIdentifiers { get; set; }
		public DbSet<Movie> Movies { get; set; }
		public DbSet<Screening> Screenings { get; set; }

		public DbSet<Reservation> Reservations { get; set; }
		public DbSet<Member> Members { get; set; }
		public DbSet<Ticket> Tickets { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Cinema>()
                .HasMany(c => c.CinemaHalls)
                .WithOne(hall => hall.Cinema)
                .HasForeignKey(hall => hall.CinemaId);
            
            modelBuilder.Entity<CinemaHall>()
                .HasMany(hall => hall.SeatIdentifiers)
                .WithOne(seat => seat.CinemaHall)
                .HasForeignKey(seat => seat.CinemaHallId);
            
            modelBuilder.Entity<CinemaHall>()
                .HasMany(hall => hall.Screenings)
                .WithOne(screening => screening.CinemaHall)
                .HasForeignKey(screening => screening.CinemaHallId);
            
            modelBuilder.Entity<Movie>()
                .HasMany(m => m.Screenings)
                .WithOne(s => s.Movie)
                .HasForeignKey(s => s.MovieId);

            modelBuilder.Entity<Member>()
                .HasMany(m => m.Tickets)
                .WithOne(t => t.Member)
                .HasForeignKey(t => t.MemberId)
                .IsRequired(false);
			
            modelBuilder.Entity<Screening>()
	            .HasMany(s => s.Reservations)
	            .WithOne(r => r.Screening)
	            .HasForeignKey(r => r.ScreeningId);
			
            modelBuilder.Entity<Member>()
	            .HasMany(m => m.Reservations)
	            .WithOne(r => r.Member)
	            .HasForeignKey(r => r.MemberId)
	            .IsRequired(false);
		}
	}
}