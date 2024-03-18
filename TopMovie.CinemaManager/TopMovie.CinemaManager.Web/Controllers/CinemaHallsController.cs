using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TopMovie.CinemaManager.Core.Features.Cinemas.Models;
using TopMovie.CinemaManager.Framework.Data;

namespace TopMovie.CinemaManager.Web.Controllers;

public class CinemaHallsController : Controller
{
	private readonly CinemaManagerDbContext _context;

	public CinemaHallsController(CinemaManagerDbContext context)
	{
		_context = context;
	}

    public async Task<IActionResult> Index(int? cinemaId)
    {
        IQueryable<CinemaHall> cinemaHallsQuery = _context.CinemaHalls.Include(c => c.Cinema);

        if (cinemaId.HasValue)
        {
            cinemaHallsQuery = cinemaHallsQuery.Where(c => c.CinemaId == cinemaId);
            ViewData["CinemaId"] = cinemaId.Value;
        }
        else
        {
            ViewData["CinemaId"] = null;
        }

        var cinemaHalls = await cinemaHallsQuery.ToListAsync();

        return View(cinemaHalls);
    }


    // GET: CinemaHalls/Details/5
    public async Task<IActionResult> Details(int? id)
	{
		if (id == null) return NotFound();

		var cinemaHall = await _context.CinemaHalls
			.Include(c => c.Cinema)
			.FirstOrDefaultAsync(m => m.Id == id);
		if (cinemaHall == null) return NotFound();

		return View(cinemaHall);
	}

	// GET: CinemaHalls/Create
	[HttpGet]
	public IActionResult Create()
	{
		ViewData["CinemaId"] = new SelectList(_context.Cinemas, "Id", "Name");
		return View();
	}

	[HttpPost]
    public async Task<IActionResult> Create([Bind("Id,Name,CinemaId,Capacity")] CinemaHall cinemaHall, [FromForm] Dictionary<string, int> rows)
    {
        var cinema = await _context.Cinemas.FindAsync(cinemaHall.CinemaId);

        if (cinema == null)
        {
            ModelState.AddModelError("CinemaId", "Invalid Cinema selected.");
            ViewData["CinemaId"] = new SelectList(_context.Cinemas, "Id", "Name", cinemaHall.CinemaId);
            return View(cinemaHall);
        }

        if (ModelState.IsValid)
        {
            foreach (var row in rows)
            {
                char rowLetter = row.Key[0];
                int seatsInRow = row.Value;

                for (int seatNumber = 1; seatNumber <= seatsInRow; seatNumber++)
                {
                    cinemaHall.SeatIdentifiers.Add(new SeatIdentifier
                    {
                        Row = rowLetter.ToString(),
                        Number = seatNumber,
                        CinemaHallId = cinemaHall.Id
                    });
                }
            }

            cinemaHall.Capacity = cinemaHall.SeatIdentifiers.Count;
            _context.Add(cinemaHall);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["CinemaId"] = new SelectList(_context.Cinemas, "Id", "Name", cinemaHall.CinemaId);
        return View(cinemaHall);
    }


	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CinemaId,Capacity")] CinemaHall cinemaHall)
	{
		if (id != cinemaHall.Id) return NotFound();

		if (ModelState.IsValid)
		{
			try
			{
				_context.Update(cinemaHall);
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!CinemaHallExists(cinemaHall.Id))
					return NotFound();
				throw;
			}

			return RedirectToAction(nameof(Index));
		}

		ViewData["CinemaId"] = new SelectList(_context.Cinemas, "Id", "Name", cinemaHall.CinemaId);
		return View(cinemaHall);
	}

	// GET: CinemaHalls/Delete/5
	public async Task<IActionResult> Delete(int? id)
	{
		if (id == null) return NotFound();

		var cinemaHall = await _context.CinemaHalls
			.Include(c => c.Cinema)
			.FirstOrDefaultAsync(m => m.Id == id);
		if (cinemaHall == null) return NotFound();

		return View(cinemaHall);
	}

	// POST: CinemaHalls/Delete/5
	[HttpPost]
	[ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(int id)
	{
		var cinemaHall = await _context.CinemaHalls.FindAsync(id);
		if (cinemaHall != null) _context.CinemaHalls.Remove(cinemaHall);

		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	private bool CinemaHallExists(int id)
	{
		return _context.CinemaHalls.Any(e => e.Id == id);
	}
}