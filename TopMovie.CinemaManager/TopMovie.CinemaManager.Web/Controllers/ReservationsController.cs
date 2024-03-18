using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TopMovie.CinemaManager.Core.Features.Cinemas.Models;
using TopMovie.CinemaManager.Core.Features.Memberships.Models;
using TopMovie.CinemaManager.Core.Features.Reservations.Models;
using TopMovie.CinemaManager.Core.Features.Screenings.Models;
using TopMovie.CinemaManager.Framework.Data;
using TopMovie.CinemaManager.Web.Models;

namespace TopMovie.CinemaManager.Web.Controllers;

[Authorize]
public class ReservationsController : Controller
{
	private readonly CinemaManagerDbContext _context;
	private readonly UserManager<Member> _userManager;

	public ReservationsController(CinemaManagerDbContext context, UserManager<Member> userManager)
	{
		_context = context;
		_userManager = userManager;
	}

	// GET: Reservations
	public async Task<IActionResult> Index()
	{
		var cinemaManagerDbContext = _context.Reservations.Include(r => r.Member).Include(r => r.Screening);
		return View(await cinemaManagerDbContext.ToListAsync());
	}

	public async Task<IActionResult> Details(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var reservation = await _context.Reservations
			.Include(r => r.Screening)
			.ThenInclude(s => s.CinemaHall)
			.ThenInclude(ch => ch.Cinema)
			.Include(r => r.Screening)
			.ThenInclude(s => s.Movie)
			.Include(r => r.ReservedSeats)
			.FirstOrDefaultAsync(m => m.ReservationId == id);

		if (reservation == null)
		{
			return NotFound();
		}

		return View(reservation);
	}

	public async Task<IActionResult> Create(int screeningId)
	{
		var screening = _context.Screenings
			.Include(s => s.CinemaHall)
			.ThenInclude(hall => hall.SeatIdentifiers) 
			.Include(s => s.CinemaHall.Cinema)
			.Include(s => s.Movie)
			.FirstOrDefault(s => s.ScreeningId == screeningId);

		if (screening == null)
		{
			return NotFound();
		}
		var cinemaHallSeats = screening.CinemaHall.SeatIdentifiers.OrderBy(si => si.Row).ThenBy(si => si.Number).ToList();
		ViewBag.CinemaHallSeats = cinemaHallSeats;

		ViewBag.ScreeningDetails = screening;
		ViewBag.ScreeningId = new SelectList(new List<Screening> { screening }, "ScreeningId", "ScreeningId", screeningId);
		ViewBag.MemberId = new SelectList(_context.Members, "Id", "Id");

		var maxRow = cinemaHallSeats.Max(si => si.Row);
		var maxSeatNumber = cinemaHallSeats.Max(si => si.Number);

		ViewBag.MaxRow = maxRow.ToCharArray().First();
		ViewBag.MaxSeatNumber = maxSeatNumber;
		ViewBag.MembershipType = await GetMembershipType();

		return View();
	}

	private async Task<string> GetMembershipType()
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		
		var reservationCount = await _context.Reservations
			.CountAsync(r => r.MemberId == userId);
		
		var membershipType = "Bronze"; // Standardtyp
		if (reservationCount >= 5 && reservationCount < 10)
		{
			membershipType = "Silber";
		}
		else if (reservationCount >= 10)
		{
			membershipType = "Gold";
		}
		return membershipType;
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(ReservationViewModel viewModel)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (ModelState.IsValid)
		{
			var reservation = new Reservation
			{
				ReservationTime = DateTime.Now,
				ScreeningId = viewModel.ScreeningId,
				MemberId = userId, 
				ReservedSeats = new List<SeatIdentifier>()
			};
			
			foreach (var seatId in viewModel.SelectedSeats)
			{
				var seat = await _context.SeatIdentifiers.FindAsync(seatId);
				if (seat == null || !seat.IsAvailable)
				{
					ModelState.AddModelError("", "Ein oder mehrere ausgewählte Sitze sind nicht verfügbar.");
					return View();
				}
				seat.IsAvailable = false;
				reservation.ReservedSeats.Add(seat);
			}

			_context.Add(reservation);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Details), new { id = reservation.ReservationId });
		}


		return RedirectToAction(nameof(Index));
	}

	public async Task<IActionResult> Edit(int? id)
	{
		if (id == null) return NotFound();

		var reservation = await _context.Reservations.FindAsync(id);
		if (reservation == null) return NotFound();
		ViewData["MemberId"] = new SelectList(_context.Members, "Id", "Id", reservation.MemberId);
		ViewData["ScreeningId"] =
			new SelectList(_context.Screenings, "ScreeningId", "ScreeningId", reservation.ScreeningId);
		return View(reservation);
	}
	
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id,
		[Bind("ReservationId,ReservationTime,ScreeningId,MemberId")]
		Reservation reservation)
	{
		if (id != reservation.ReservationId) return NotFound();

		if (ModelState.IsValid)
		{
			try
			{
				_context.Update(reservation);
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ReservationExists(reservation.ReservationId))
					return NotFound();
				throw;
			}

			return RedirectToAction(nameof(Index));
		}

		ViewData["MemberId"] = new SelectList(_context.Members, "Id", "Id", reservation.MemberId);
		ViewData["ScreeningId"] =
			new SelectList(_context.Screenings, "ScreeningId", "ScreeningId", reservation.ScreeningId);
		return View(reservation);
	}

	// GET: Reservations/Delete/5
	public async Task<IActionResult> Delete(int? id)
	{
		if (id == null) return NotFound();

		var reservation = await _context.Reservations
			.Include(r => r.Member)
			.Include(r => r.Screening)
			.FirstOrDefaultAsync(m => m.ReservationId == id);
		if (reservation == null) return NotFound();

		return View(reservation);
	}

	// POST: Reservations/Delete/5
	[HttpPost]
	[ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(int id)
	{
		var reservation = await _context.Reservations.FindAsync(id);
		if (reservation != null) _context.Reservations.Remove(reservation);

		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	private bool ReservationExists(int id)
	{
		return _context.Reservations.Any(e => e.ReservationId == id);
	}

	public async Task<IActionResult> Analytic()
	{
		var occupancyByCinemaAndHall = await _context.Reservations
			.Include(r => r.Screening)
				.ThenInclude(s => s.CinemaHall)
					.ThenInclude(ch => ch.Cinema)
			.GroupBy(r => new { CinemaName = r.Screening.CinemaHall.Cinema.Name, CinemaHallName = r.Screening.CinemaHall.Name })
			.Select(group => new AnalyticsViewModel.OccupancyData
			{
				CinemaName = group.Key.CinemaName,
				CinemaHallName = group.Key.CinemaHallName,
				Occupancy = group.Count()
			})
			.ToListAsync();

        var revenueByCinema = await _context.Screenings
            .Include(s => s.CinemaHall)
            .ThenInclude(ch => ch.Cinema)
            .Include(s => s.Reservations)
            .SelectMany(s => s.Reservations, (screening, reservation) => new
            {
                CinemaName = screening.CinemaHall.Cinema.Name,
                ScreeningPrice = screening.Price
            })
            .GroupBy(s => s.CinemaName)
            .Select(group => new AnalyticsViewModel.RevenueData
            {
                Name = group.Key,
                Revenue = group.Sum(g => g.ScreeningPrice)
            })
            .ToListAsync();

        var revenueByCinemaHall = await _context.Screenings
            .Include(s => s.CinemaHall)
            .Include(s => s.Reservations)
            .SelectMany(s => s.Reservations, (screening, reservation) => new
            {
                CinemaHallName = screening.CinemaHall.Name,
                ScreeningPrice = screening.Price
            })
            .GroupBy(s => s.CinemaHallName)
            .Select(group => new AnalyticsViewModel.RevenueData
            {
                Name = group.Key,
                Revenue = group.Sum(g => g.ScreeningPrice)
            })
            .ToListAsync();
		
        var revenueByMovie = await _context.Reservations
            .Include(r => r.Screening)
            .ThenInclude(s => s.Movie)
            .Select(r => new
            {
                MovieTitle = r.Screening.Movie.Title,
                Revenue = r.Screening.Price
            })
            .GroupBy(r => r.MovieTitle)
            .Select(group => new AnalyticsViewModel.RevenueData
            {
                Name = group.Key,
                Revenue = group.Sum(g => g.Revenue)
            })
            .ToListAsync();

        var revenueByUser = await _context.Reservations
			.Include(r => r.Member)
			.GroupBy(r => r.MemberId)
			.Select(group => new AnalyticsViewModel.UserRevenueData
			{
				MemberId = group.Key,
				Revenue = group.Sum(g => g.Screening.Price)
			})
			.ToListAsync();

		var viewModel = new AnalyticsViewModel
		{
			OccupancyByCinemaAndHall = occupancyByCinemaAndHall,
			RevenueByCinema = revenueByCinema,
			RevenueByCinemaHall = revenueByCinemaHall,
			RevenueByMovie = revenueByMovie,
			RevenueByUser = revenueByUser
		};

		return View(viewModel);
	}
}