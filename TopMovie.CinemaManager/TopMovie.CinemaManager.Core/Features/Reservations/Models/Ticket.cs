using TopMovie.CinemaManager.Core.Features.Memberships.Models;

namespace TopMovie.CinemaManager.Core.Features.Reservations.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int ScreeningId { get; set; }
        public string MemberId { get; set; }
        public int SeatId { get; set; }
        public DateTime PurchaseDate { get; set; }

        public Member Member { get; set; }
        
		public decimal Price { get; set; }
    }
}