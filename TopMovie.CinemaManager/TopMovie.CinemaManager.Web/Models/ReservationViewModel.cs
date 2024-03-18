namespace TopMovie.CinemaManager.Web.Models;

public class ReservationViewModel
{
	public int ScreeningId { get; set; }
	public List<int> SelectedSeats { get; set; } = new();
}