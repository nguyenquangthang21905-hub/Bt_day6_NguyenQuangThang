using Bt_day6_NguyenQuangThang.Data;
using Bt_day6_NguyenQuangThang.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bt_day6_NguyenQuangThang.Controllers;

public class MoviesController : Controller
{
    private readonly ApplicationDbContext _context;

    public MoviesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var movies = await _context.Movies.Include(movie => movie.Genre)
            .OrderBy(movie => movie.Title).ToListAsync();
        return View(movies);
    }

    public async Task<IActionResult> Create()
    {
        await LoadGenresAsync();
        return View(new Movie());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,ReleaseDate,Duration,GenreId")] Movie movie)
    {
        if (!await _context.Genres.AnyAsync(genre => genre.Id == movie.GenreId))
        {
            ModelState.AddModelError(nameof(movie.GenreId), "Thể loại đã chọn không tồn tại.");
        }

        if (!ModelState.IsValid)
        {
            await LoadGenresAsync(movie.GenreId);
            return View(movie);
        }

        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var movie = await _context.Movies.FindAsync(id.Value);
        if (movie is null)
        {
            return NotFound();
        }

        await LoadGenresAsync(movie.GenreId);
        return View(movie);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseDate,Duration,GenreId")] Movie movie)
    {
        if (id != movie.Id)
        {
            return NotFound();
        }

        var existingMovie = await _context.Movies.FindAsync(id);
        if (existingMovie is null)
        {
            return NotFound();
        }

        if (!await _context.Genres.AnyAsync(genre => genre.Id == movie.GenreId))
        {
            ModelState.AddModelError(nameof(movie.GenreId), "Thể loại đã chọn không tồn tại.");
        }

        if (!ModelState.IsValid)
        {
            await LoadGenresAsync(movie.GenreId);
            return View(movie);
        }

        existingMovie.Title = movie.Title;
        existingMovie.ReleaseDate = movie.ReleaseDate;
        existingMovie.Duration = movie.Duration;
        existingMovie.GenreId = movie.GenreId;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var movie = await _context.Movies.Include(item => item.Genre)
            .FirstOrDefaultAsync(item => item.Id == id.Value);
        return movie is null ? NotFound() : View(movie);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie is null)
        {
            return NotFound();
        }

        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadGenresAsync(int? selectedGenreId = null)
    {
        ViewBag.GenreId = new SelectList(
            await _context.Genres.OrderBy(genre => genre.Name).ToListAsync(),
            nameof(Genre.Id), nameof(Genre.Name), selectedGenreId);
    }
}