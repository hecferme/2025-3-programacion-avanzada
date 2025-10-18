using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Repositories DI - prefer EF implementations; if you want to use JSON file-based repos
// you can register the Json*Repository implementations instead. The Data folder
// contains JSON test files (authors.json, books.json, themes.json, bookauthors.json, bookthemes.json).
var useJson = builder.Configuration.GetValue<bool>("UseJsonSources");
var conn = builder.Configuration.GetConnectionString("user00");

if (useJson)
{
    var dataPath = System.IO.Path.Combine(builder.Environment.ContentRootPath, "Data");
    builder.Services.AddScoped<ProgramacionAvanzada.Books.Repositories.IAuthorRepository>(_ => new ProgramacionAvanzada.Books.Repositories.JsonAuthorRepository(System.IO.Path.Combine(dataPath, "authors.json")));
    builder.Services.AddScoped<ProgramacionAvanzada.Books.Repositories.IBookRepository>(_ => new ProgramacionAvanzada.Books.Repositories.JsonBookRepository(System.IO.Path.Combine(dataPath, "books.json")));
    builder.Services.AddScoped<ProgramacionAvanzada.Books.Repositories.IThemeRepository>(_ => new ProgramacionAvanzada.Books.Repositories.JsonThemeRepository(System.IO.Path.Combine(dataPath, "themes.json")));
    builder.Services.AddScoped<ProgramacionAvanzada.Books.Repositories.IBookAuthorRepository>(_ => new ProgramacionAvanzada.Books.Repositories.JsonBookAuthorRepository(System.IO.Path.Combine(dataPath, "bookauthors.json"), System.IO.Path.Combine(dataPath, "books.json"), System.IO.Path.Combine(dataPath, "authors.json")));
    builder.Services.AddScoped<ProgramacionAvanzada.Books.Repositories.IBookThemeRepository>(_ => new ProgramacionAvanzada.Books.Repositories.JsonBookThemeRepository(System.IO.Path.Combine(dataPath, "bookthemes.json"), System.IO.Path.Combine(dataPath, "books.json"), System.IO.Path.Combine(dataPath, "themes.json")));
    // Borrow JSON repo is available now
    builder.Services.AddScoped<ProgramacionAvanzada.Books.Repositories.IBorrowRepository>(_ => new ProgramacionAvanzada.Books.Repositories.JsonBorrowRepository(System.IO.Path.Combine(dataPath, "borrows.json"), System.IO.Path.Combine(dataPath, "bookcopies.json"), System.IO.Path.Combine(dataPath, "authors.json")));
}
else if (!string.IsNullOrWhiteSpace(conn))
{
    // Register the DbContext - provider configuration is left to the environment
    builder.Services.AddDbContext<ProgramacionAvanzada.Books.Model.BooksDbContext>();

    // Register EF repositories
    builder.Services.AddScoped<ProgramacionAvanzada.Books.Repositories.IAuthorRepository, ProgramacionAvanzada.Books.Repositories.EfAuthorRepository>();
    builder.Services.AddScoped<ProgramacionAvanzada.Books.Repositories.IBookRepository, ProgramacionAvanzada.Books.Repositories.EfBookRepository>();
    builder.Services.AddScoped<ProgramacionAvanzada.Books.Repositories.IBookAuthorRepository, ProgramacionAvanzada.Books.Repositories.EfBookAuthorRepository>();
    builder.Services.AddScoped<ProgramacionAvanzada.Books.Repositories.IThemeRepository, ProgramacionAvanzada.Books.Repositories.EfThemeRepository>();
    builder.Services.AddScoped<ProgramacionAvanzada.Books.Repositories.IBookThemeRepository, ProgramacionAvanzada.Books.Repositories.EfBookThemeRepository>();
    // Borrow repository will use EF implementation added to project
    builder.Services.AddScoped<ProgramacionAvanzada.Books.Repositories.IBorrowRepository, ProgramacionAvanzada.Books.Repositories.EfBorrowRepository>();
}
else
{
    // No connection string and JSON mode disabled: fall back to JSON repositories as best-effort
    var dataPath = System.IO.Path.Combine(builder.Environment.ContentRootPath, "Data");
    builder.Services.AddScoped<ProgramacionAvanzada.Books.Repositories.IAuthorRepository>(_ => new ProgramacionAvanzada.Books.Repositories.JsonAuthorRepository(System.IO.Path.Combine(dataPath, "authors.json")));
    builder.Services.AddScoped<ProgramacionAvanzada.Books.Repositories.IBookRepository>(_ => new ProgramacionAvanzada.Books.Repositories.JsonBookRepository(System.IO.Path.Combine(dataPath, "books.json")));
    builder.Services.AddScoped<ProgramacionAvanzada.Books.Repositories.IThemeRepository>(_ => new ProgramacionAvanzada.Books.Repositories.JsonThemeRepository(System.IO.Path.Combine(dataPath, "themes.json")));
    builder.Services.AddScoped<ProgramacionAvanzada.Books.Repositories.IBookAuthorRepository>(_ => new ProgramacionAvanzada.Books.Repositories.JsonBookAuthorRepository(System.IO.Path.Combine(dataPath, "bookauthors.json"), System.IO.Path.Combine(dataPath, "books.json"), System.IO.Path.Combine(dataPath, "authors.json")));
    builder.Services.AddScoped<ProgramacionAvanzada.Books.Repositories.IBookThemeRepository>(_ => new ProgramacionAvanzada.Books.Repositories.JsonBookThemeRepository(System.IO.Path.Combine(dataPath, "bookthemes.json"), System.IO.Path.Combine(dataPath, "books.json"), System.IO.Path.Combine(dataPath, "themes.json")));
    builder.Services.AddScoped<ProgramacionAvanzada.Books.Repositories.IBorrowRepository>(_ => new ProgramacionAvanzada.Books.Repositories.JsonBorrowRepository(System.IO.Path.Combine(dataPath, "borrows.json"), System.IO.Path.Combine(dataPath, "bookcopies.json"), System.IO.Path.Combine(dataPath, "authors.json")));
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
