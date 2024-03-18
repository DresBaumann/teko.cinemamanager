using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TopMovie.CinemaManager.Core.Features.Cinemas.Models;
using TopMovie.CinemaManager.Framework.Data;

namespace TopMovie.CinemaManager.Web.Controllers;

public class CinemasController : Controller
{
	private readonly CinemaManagerDbContext _context;

	public CinemasController(CinemaManagerDbContext context)
	{
		_context = context;
	}

	// GET: Cinemas
	public async Task<IActionResult> Index()
	{
		return View(await _context.Cinemas.ToListAsync());
	}

	// GET: Cinemas/Details/5
	public async Task<IActionResult> Details(int? id)
	{
		if (id == null) return NotFound();

		var cinema = await _context.Cinemas
			.FirstOrDefaultAsync(m => m.Id == id);
		if (cinema == null) return NotFound();

		return View(cinema);
	}

	// GET: Cinemas/Create
	public IActionResult Create()
	{
		return View();
	}

	// POST: Cinemas/Create
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create([Bind("Id,Name,StreetName,StreetNumber,PLZ")] Cinema cinema)
	{
		if (ModelState.IsValid)
		{
			_context.Add(cinema);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		return View(cinema);
	}

	// GET: Cinemas/Edit/5
	public async Task<IActionResult> Edit(int? id)
	{
		if (id == null) return NotFound();

		var cinema = await _context.Cinemas.FindAsync(id);
		if (cinema == null) return NotFound();
		return View(cinema);
	}

	// POST: Cinemas/Edit/5
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StreetName,StreetNumber,PLZ")] Cinema cinema)
	{
		if (id != cinema.Id) return NotFound();

		if (ModelState.IsValid)
		{
			try
			{
				_context.Update(cinema);
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!CinemaExists(cinema.Id))
					return NotFound();
				throw;
			}

			return RedirectToAction(nameof(Index));
		}

		return View(cinema);
	}

	// GET: Cinemas/Delete/5
	public async Task<IActionResult> Delete(int? id)
	{
		if (id == null) return NotFound();

		var cinema = await _context.Cinemas
			.FirstOrDefaultAsync(m => m.Id == id);
		if (cinema == null) return NotFound();

		return View(cinema);
	}

	// POST: Cinemas/Delete/5
	[HttpPost]
	[ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(int id)
	{
		var cinema = await _context.Cinemas.FindAsync(id);
		if (cinema != null) _context.Cinemas.Remove(cinema);

		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	private bool CinemaExists(int id)
	{
		return _context.Cinemas.Any(e => e.Id == id);
	}
}