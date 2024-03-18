namespace TopMovie.CinemaManager.Web.Models;

public class AnalyticsViewModel
{
	public List<OccupancyData> OccupancyByCinemaAndHall { get; set; }
	public List<RevenueData> RevenueByCinema { get; set; }
	public List<RevenueData> RevenueByCinemaHall { get; set; }
	public List<RevenueData> RevenueByMovie { get; set; }
	public List<UserRevenueData> RevenueByUser { get; set; }

	public class OccupancyData
	{
		public string CinemaName { get; set; }
		public string CinemaHallName { get; set; }
		public int Occupancy { get; set; }
	}

	public class RevenueData
	{
		public string Name { get; set; }
		public decimal Revenue { get; set; }
	}

	public class UserRevenueData
	{
		public string MemberId { get; set; }
		public decimal Revenue { get; set; }
	}
}