using System.ComponentModel.DataAnnotations;
using TopMovie.CinemaManager.Core.Features.Screenings.Models;

namespace TopMovie.CinemaManager.Core.Features.Cinemas.Models;

public class CinemaHall
{
	[Key] public int Id { get; set; }

	[Required] public string Name { get; set; }

	public int CinemaId { get; set; }
	public Cinema? Cinema { get; set; }

	// Kapazität könnte automatisch aus der Anzahl der SeatIdentifiers berechnet werden,
	// hier jedoch explizit als Information.
	public int Capacity { get; set; }

	public ICollection<SeatIdentifier> SeatIdentifiers { get; set; } = new List<SeatIdentifier>();

	public ICollection<Screening> Screenings { get; set; } = new List<Screening>();
}