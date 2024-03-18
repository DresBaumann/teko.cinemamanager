using Microsoft.AspNetCore.Identity;
using TopMovie.CinemaManager.Core.Features.Reservations.Models;

namespace TopMovie.CinemaManager.Core.Features.Memberships.Models;

public class Member : IdentityUser
{
	public enum MembershipStatus
	{
		Bronze,
		Silver,
		Gold
	}

	public MembershipStatus Status { get; set; }
	public DateTime RegistrationDate { get; set; }
	public ICollection<Ticket> Tickets { get; set; }

	public ICollection<Reservation> Reservations { get; set; }
}