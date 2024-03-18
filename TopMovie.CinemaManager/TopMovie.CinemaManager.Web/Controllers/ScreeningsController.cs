using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TopMovie.CinemaManager.Core.Features.Screenings.Models;
using TopMovie.CinemaManager.Framework.Data;

namespace TopMovie.CinemaManager.Web.Controllers;

public class ScreeningsController : Controller
{
	private readonly CinemaManagerDbContext _context;

	public ScreeningsController(CinemaManagerDbContext context)
	{
		_context = context;
	}
    public async Task<IActionResult> Index(int? hallId, DateTime? screeningDate)
    {
        IQueryable<Screening> cinemaManagerDbContext = _context.Screenings.Include(s => s.CinemaHall).Include(s => s.Movie);

        if (hallId.HasValue)
        {
            ViewBag.HallId = hallId;
            cinemaManagerDbContext = cinemaManagerDbContext.Where(s => s.CinemaHallId == hallId.Value);
        }

        if (!screeningDate.HasValue)
        {
            screeningDate = DateTime.Today;
        }

        cinemaManagerDbContext = cinemaManagerDbContext.Where(s => s.StartTime.Date == screeningDate.Value.Date);

        var standardTimes = new List<TimeSpan>
        {
            new TimeSpan(14, 15, 0),
            new TimeSpan(17, 15, 0),
            new TimeSpan(20, 15, 0)
        };

        if (screeningDate.Value.DayOfWeek == DayOfWeek.Saturday || screeningDate.Value.DayOfWeek == DayOfWeek.Sunday)
        {
            standardTimes.Add(new TimeSpan(23, 0, 0));
        }

        var screeningsForDate = await cinemaManagerDbContext.ToListAsync();
        var missingTimes = standardTimes.Where(time => !screeningsForDate.Any(s => s.StartTime.TimeOfDay == time)).ToList();

        ViewBag.MissingTimes = missingTimes;
        ViewBag.StandardTimes = standardTimes;
        ViewBag.SelectedDate = screeningDate.Value;

        return View(screeningsForDate);
    }

    public async Task<IActionResult> Program(int? cinemaId, int? movieId, DateTime? screeningDate, string viewOption)
    {
        var screeningsQuery = _context.Screenings
            .Include(s => s.CinemaHall)
            .Include(s => s.Movie)
            .AsQueryable();

        if (cinemaId.HasValue && cinemaId.Value > 0)
        {
            screeningsQuery = screeningsQuery.Where(s => s.CinemaHall.CinemaId == cinemaId.Value);
        }

        if (movieId.HasValue && movieId.Value != 0)
        {
	        screeningsQuery = screeningsQuery.Where(s => s.MovieId == movieId.Value);
        }

		if (screeningDate.HasValue)
        {
            if (viewOption == "day")
            {
                screeningsQuery = screeningsQuery.Where(s => s.StartTime.Date == screeningDate.Value.Date);
            }
            else if (viewOption == "week")
            {
                var endDate = screeningDate.Value.AddDays(7);
                screeningsQuery = screeningsQuery.Where(s => s.StartTime.Date >= screeningDate.Value.Date && s.StartTime.Date <= endDate);
            }
        }

        var screenings = await screeningsQuery.ToListAsync();

        // Laden Sie die Kinos für das Dropdown
        ViewBag.Cinemas = await _context.Cinemas.ToListAsync();
        ViewBag.SelectedCinemaId = cinemaId.HasValue ? cinemaId.Value : 0;
        ViewBag.SelectedMovieId = movieId.HasValue ? movieId.Value : 0;
		ViewBag.SelectedViewOption = viewOption ?? "day";
        ViewBag.Movies = await _context.Movies.ToListAsync();
		ViewBag.SelectedDate = screeningDate.HasValue ? screeningDate.Value.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd");


		return View(screenings);
    }

    // GET: Screenings/Details/5
    public async Task<IActionResult> Details(int? id)
	{
		if (id == null) return NotFound();

		var screening = await _context.Screenings
			.Include(s => s.CinemaHall)
			.Include(s => s.Movie)
			.FirstOrDefaultAsync(m => m.ScreeningId == id);
		if (screening == null) return NotFound();

		return View(screening);
	}

    public IActionResult Create(DateTime? screeningDate, int? hallId)
    {
        var screening = new Screening();
        if (screeningDate.HasValue )
        {
            screening.StartTime = screeningDate.Value;
        }

        if (hallId.HasValue)
        {
            screening.CinemaHallId = hallId.Value;
            ViewData["CinemaHallId"] = new SelectList(_context.CinemaHalls, "Id", "Name", hallId.Value);
        }
        else
        {
            ViewData["CinemaHallId"] = new SelectList(_context.CinemaHalls, "Id", "Name");
        }

        ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "Title");

        return View(screening);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("CinemaHallId,MovieId,Price,StartDate,StartTime")] Screening model)
    {
        // Überprüfen, ob das Kino und der Film existieren
        var cinemaHallExists = await _context.CinemaHalls.AnyAsync(ch => ch.Id == model.CinemaHallId);
        var movieExists = await _context.Movies.AnyAsync(m => m.MovieId == model.MovieId);

        if (!cinemaHallExists)
        {
            ModelState.AddModelError("CinemaHallId", "Das ausgewählte Kino existiert nicht.");
        }

        if (!movieExists)
        {
            ModelState.AddModelError("MovieId", "Der ausgewählte Film existiert nicht.");
        }

        if (ModelState.IsValid)
        {
            var screening = new Screening
            {
                CinemaHallId = model.CinemaHallId,
                MovieId = model.MovieId,
                Price = model.Price,
                StartTime = model.StartTime
            };

            _context.Add(screening);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["CinemaHallId"] = new SelectList(_context.CinemaHalls, "Id", "Name", model.CinemaHallId);
        ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "Title", model.MovieId);
        return View(model);
    }



    // GET: Screenings/Edit/5
    public async Task<IActionResult> Edit(int? id)
	{
		if (id == null) return NotFound();

		var screening = await _context.Screenings.FindAsync(id);
		if (screening == null) return NotFound();
		ViewData["CinemaHallId"] = new SelectList(_context.CinemaHalls, "Id", "Name", screening.CinemaHallId);
		ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "Title", screening.MovieId);
		return View(screening);
	}

	// POST: Screenings/Edit/5
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id,
		[Bind("ScreeningId,StartTime,CinemaHallId,MovieId,Price")] Screening screening)
	{
		if (id != screening.ScreeningId) return NotFound();

		if (ModelState.IsValid)
		{
			try
			{
				_context.Update(screening);
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ScreeningExists(screening.ScreeningId))
					return NotFound();
				throw;
			}

			return RedirectToAction(nameof(Index));
		}

		ViewData["CinemaHallId"] = new SelectList(_context.CinemaHalls, "Id", "Name", screening.CinemaHallId);
		ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "Title", screening.MovieId);
		return View(screening);
	}

	// GET: Screenings/Delete/5
	public async Task<IActionResult> Delete(int? id)
	{
		if (id == null) return NotFound();

		var screening = await _context.Screenings
			.Include(s => s.CinemaHall)
			.Include(s => s.Movie)
			.FirstOrDefaultAsync(m => m.ScreeningId == id);
		if (screening == null) return NotFound();

		return View(screening);
	}

	// POST: Screenings/Delete/5
	[HttpPost]
	[ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(int id)
	{
		var screening = await _context.Screenings.FindAsync(id);
		if (screening != null) _context.Screenings.Remove(screening);

		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	private bool ScreeningExists(int id)
	{
		return _context.Screenings.Any(e => e.ScreeningId == id);
	}
}