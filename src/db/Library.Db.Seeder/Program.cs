using Bogus;
using Library.Db;
using Library.Db.Entities.Library;
using Library.Db.Entities.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder();

builder.Services.AddLibraryDb(
    connectionString: builder.Configuration.GetConnectionString("LibraryDb")!
);

await using var app = builder.Build();

var factory = app.Services.GetRequiredService<IDbContextFactory<LibraryDbContext>>();
await using var db = factory.CreateDbContext();

// Seed only if the database is empty
if (await db.Books.AnyAsync() || await db.Authors.AnyAsync() || await db.Users.AnyAsync())
{
    app.Logger.LogInformation("Database already seeded. Skipping.");
    return;
}


Randomizer.Seed = new Random(8675309);

// Users
var userFaker = new Faker<UserDbo>(locale: "it")
    .RuleFor(x => x.Id, _ => Guid.NewGuid())
    .RuleFor(x => x.Email, f => f.Internet.Email().ToLower());

var users = userFaker.Generate(30);

// Authors
var authorFaker = new Faker<AuthorDbo>(locale: "it")
    .RuleFor(x => x.Id, _ => Guid.NewGuid())
    .RuleFor(x => x.FirstName, f => f.Name.FirstName())
    .RuleFor(x => x.LastName, f => f.Name.LastName())
    .RuleFor(x => x.BirthDate, f =>
    {
        var d = f.Date.BetweenDateOnly(new DateOnly(1930, 1, 1), new DateOnly(2005, 12, 31));
        return f.Random.Bool(85) ? d : null;
    })
    .RuleFor(x => x.Biography, f => f.Random.Bool(40) ? f.Lorem.Paragraphs(1, 3) : null)
    .RuleFor(x => x.Country, f => f.Random.Bool(70) ? f.Address.Country() : null)
    .RuleFor(x => x.City, f => f.Random.Bool(70) ? f.Address.City() : null)
    .RuleFor(x => x.PictureUrl, f => f.Random.Bool(30) ? f.Image.PicsumUrl() : null);

var authors = authorFaker.Generate(50);

// Books
var usedEans = new HashSet<string>();

string NextEan(Faker f)
{
    string ean;
    do
    {
        ean = f.Random.ReplaceNumbers("#############"); // 13 digits
        if (ean.Length < 13)
            ean = ean.PadLeft(13, '0');
    } while (!usedEans.Add(ean));

    return ean;
}

var bookFaker = new Faker<BookDbo>(locale: "it")
    .RuleFor(x => x.Id, _ => Guid.NewGuid())
    .RuleFor(x => x.Ean, (f, _) => NextEan(f))
    .RuleFor(x => x.Title, f => f.Lorem.Sentence(3, 5))
    .RuleFor(x => x.Description, f => f.Random.Bool(60) ? f.Lorem.Paragraphs(1, 3) : null)
    .RuleFor(x => x.Year,
        f => f.Random.Bool(80)
            ? f.Date.BetweenDateOnly(new DateOnly(1950, 1, 1), DateOnly.FromDateTime(DateTime.UtcNow)).Year
            : null)
    .RuleFor(x => x.AvailableCopies, f => f.Random.Int(0, 25));

var books = bookFaker.Generate(100);

// Link authors to books (many-to-many)
var rnd = new Random(42);
foreach (var book in books)
{
    var count = rnd.Next(1, 4); // 1-3 authors per book
    var chosen = authors.OrderBy(_ => rnd.Next()).Take(count);
    foreach (var a in chosen)
    {
        book.Authors.Add(a);
        a.Books.Add(book);
    }
}

await db.Users.AddRangeAsync(users);
await db.Authors.AddRangeAsync(authors);
await db.Books.AddRangeAsync(books);
await db.SaveChangesAsync();

// Borrow requests (after books and users exist)
var statusValues = Enum.GetValues<BorrowStatus>();
var borrowFaker = new Faker<BorrowRequestDbo>(locale: "it")
    .RuleFor(x => x.Id, _ => Guid.NewGuid())
    .RuleFor(x => x.UserId, f => f.PickRandom(users).Id)
    .RuleFor(x => x.BookId, f => f.PickRandom(books).Id)
    .RuleFor(x => x.Status, f => f.PickRandom(statusValues))
    .RuleFor(x => x.RequestedAt,
        f => f.Date.BetweenOffset(DateTimeOffset.UtcNow.AddMonths(-6), DateTimeOffset.UtcNow))
    .RuleFor(x => x.AcceptedAt, (f, br) =>
    {
        if (br.Status is BorrowStatus.Accepted or BorrowStatus.Returned)
            return br.RequestedAt.AddDays(f.Random.Int(0, 7));
        return null;
    })
    .RuleFor(x => x.ReturnedAt, (f, br) =>
    {
        if (br.Status == BorrowStatus.Returned && br.AcceptedAt.HasValue)
            return br.AcceptedAt.Value.AddDays(f.Random.Int(1, 30));
        return null;
    });

var borrowRequests = borrowFaker.Generate(200);
await db.BorrowRequests.AddRangeAsync(borrowRequests);
await db.SaveChangesAsync();

app.Logger.LogInformation(
    "Seed completed: Users={UsersCount}, Authors={AuthorsCount}, Books={BooksCount}, BorrowRequests={BorrowRequestsCount}",
    users.Count,
    authors.Count,
    books.Count,
    borrowRequests.Count
);