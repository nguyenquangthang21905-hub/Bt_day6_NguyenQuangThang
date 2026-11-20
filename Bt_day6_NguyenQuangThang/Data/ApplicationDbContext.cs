using Bt_day6_NguyenQuangThang.Models;
using Microsoft.EntityFrameworkCore;

namespace Bt_day6_NguyenQuangThang.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Movie> Movies => Set<Movie>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Genre>()
            .HasMany(genre => genre.Movies)
            .WithOne(movie => movie.Genre)
            .HasForeignKey(movie => movie.GenreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}