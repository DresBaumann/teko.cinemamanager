using TopMovie.CinemaManager.Core.Features.Cinemas.Models;
using TopMovie.CinemaManager.Core.Features.Memberships.Models;
using TopMovie.CinemaManager.Core.Features.Screenings.Models;

namespace TopMovie.CinemaManager.Core.Features.Reservations.Models;

public class Reservation
{
	public int ReservationId { get; set; }
	public DateTime ReservationTime { get; set; }
	public int ScreeningId { get; set; }
	public Screening? Screening { get; set; }

	public string MemberId { get; set; }
	public Member? Member { get; set; }

	public ICollection<SeatIdentifier> ReservedSeats { get; set; }
}