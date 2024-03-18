using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TopMovie.CinemaManager.Core.Features.Cinemas.Models;
using TopMovie.CinemaManager.Core.Features.Reservations.Models;

namespace TopMovie.CinemaManager.Core.Features.Screenings.Models; 

public class Screening
{
	public Screening()
	{
		Reservations = new List<Reservation>();
	}

	[Key] public int ScreeningId { get; set; }

	[Required] public DateTime StartTime { get; set; }
	
	[ForeignKey("CinemaHall")] public int CinemaHallId { get; set; }

	public CinemaHall? CinemaHall { get; set; }
	
	[ForeignKey("Movie")] public int MovieId { get; set; }

	public Movie? Movie { get; set; }
	
	public decimal Price { get; set; }
	
	public ICollection<Reservation> Reservations { get; set; }
}