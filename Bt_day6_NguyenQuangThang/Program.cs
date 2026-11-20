using Bt_day6_NguyenQuangThang.Data;
using Bt_day6_NguyenQuangThang.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Movies}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();

    var sampleGenres = new[] { "Hành động", "Hoạt hình", "Kinh dị", "Tình cảm" };
    foreach (var name in sampleGenres)
    {
        if (!await dbContext.Genres.AnyAsync(genre => genre.Name == name))
        {
            dbContext.Genres.Add(new Genre { Name = name });
        }
    }

    await dbContext.SaveChangesAsync();

    var sampleMovies = new[]
    {
        (Title: "John Wick", Genre: "Hành động", Year: 2014, Duration: 101),
        (Title: "Doraemon", Genre: "Hoạt hình", Year: 2000, Duration: 90),
        (Title: "The Conjuring", Genre: "Kinh dị", Year: 2013, Duration: 112)
    };

    foreach (var sample in sampleMovies)
    {
        if (!await dbContext.Movies.AnyAsync(movie => movie.Title == sample.Title))
        {
            var genre = await dbContext.Genres.SingleAsync(item => item.Name == sample.Genre);
            dbContext.Movies.Add(new Movie
            {
                Title = sample.Title,
                ReleaseDate = new DateTime(sample.Year, 1, 1),
                Duration = sample.Duration,
                GenreId = genre.Id
            });
        }
    }

    await dbContext.SaveChangesAsync();
}

app.Run();
