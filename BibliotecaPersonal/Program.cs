using BibliotecaPersonal.Data;
using BibliotecaPersonal.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configurar SQLite
builder.Services.AddDbContext<BibliotecaDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=biblioteca.db"));

// Registrar servicios
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddHttpClient<IBookExtractionService, GeminiBookExtractionService>();

var app = builder.Build();

// Aplicar migraciones automáticamente en desarrollo
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<BibliotecaDbContext>();
    dbContext.Database.EnsureCreated();

    // Poblar con datos de ejemplo
    await BookSeeder.SeedAsync(dbContext);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();

// Configurar Minimal APIs para el backend
var apiGroup = app.MapGroup("/api");

// Endpoints de libros
apiGroup.MapGet("/books", async (IBookService bookService) =>
{
    var books = await bookService.GetAllBooksAsync();
    return Results.Ok(books);
});

apiGroup.MapGet("/books/{id:guid}", async (Guid id, IBookService bookService) =>
{
    var book = await bookService.GetBookByIdAsync(id);
    return book is not null ? Results.Ok(book) : Results.NotFound();
});

apiGroup.MapPost("/books", async (Book book, IBookService bookService) =>
{
    var created = await bookService.CreateBookAsync(book);
    return Results.Created($"/api/books/{created.Id}", created);
});

apiGroup.MapPut("/books/{id:guid}", async (Guid id, Book book, IBookService bookService) =>
{
    if (id != book.Id)
        return Results.BadRequest("ID mismatch");

    var updated = await bookService.UpdateBookAsync(book);
    return updated is not null ? Results.Ok(updated) : Results.NotFound();
});

apiGroup.MapDelete("/books/{id:guid}", async (Guid id, IBookService bookService) =>
{
    var deleted = await bookService.DeleteBookAsync(id);
    return deleted ? Results.NoContent() : Results.NotFound();
});

apiGroup.MapGet("/books/search", async (string? query, IBookService bookService) =>
{
    if (string.IsNullOrWhiteSpace(query))
        return Results.Ok(await bookService.GetAllBooksAsync());

    var books = await bookService.SearchBooksAsync(query);
    return Results.Ok(books);
});

app.MapRazorComponents<BibliotecaPersonal.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
