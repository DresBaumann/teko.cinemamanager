using System.ComponentModel.DataAnnotations;

namespace TopMovie.CinemaManager.Core.Features.Screenings.Models;

public class Movie
{
	[Key] public int MovieId { get; set; }

	[Required] public string Title { get; set; }

	public TimeSpan Duration { get; set; }

	[Required] public string Genre { get; set; }

	public string Director { get; set; }
	public string Description { get; set; }
	public DateTime ReleaseDate { get; set; }

	public ICollection<Screening> Screenings { get; set; } = new List<Screening>();
}