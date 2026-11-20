using Bt_day6_NguyenQuangThang.Data;
using Bt_day6_NguyenQuangThang.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bt_day6_NguyenQuangThang.Controllers;

public class GenresController : Controller
{
    private readonly ApplicationDbContext _context;

    public GenresController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Genres.OrderBy(genre => genre.Name).ToListAsync());
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name")] Genre genre)
    {
        if (!ModelState.IsValid)
        {
            return View(genre);
        }

        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var genre = await _context.Genres.FindAsync(id.Value);
        return genre is null ? NotFound() : View(genre);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Genre genre)
    {
        if (id != genre.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(genre);
        }

        var existingGenre = await _context.Genres.FindAsync(id);
        if (existingGenre is null)
        {
            return NotFound();
        }

        existingGenre.Name = genre.Name;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var genre = await _context.Genres.Include(item => item.Movies)
            .FirstOrDefaultAsync(item => item.Id == id.Value);
        return genre is null ? NotFound() : View(genre);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var genre = await _context.Genres.FindAsync(id);
        if (genre is null)
        {
            return NotFound();
        }

        if (await _context.Movies.AnyAsync(movie => movie.GenreId == id))
        {
            TempData["ErrorMessage"] = "Không thể xóa thể loại đang có phim.";
            return RedirectToAction(nameof(Index));
        }

        _context.Genres.Remove(genre);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}