using System.ComponentModel;
using Library.Db;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;

namespace Library.Mcp.Public.Server.Tools.Books.Get;

[McpServerToolType]
public sealed class GetBookTool
{
    private readonly LibraryDbContext _db;

    public GetBookTool(LibraryDbContext db)
    {
        _db = db;
    }

    [McpServerTool(
        Name = "get_book",
        Title = "Get book",
        Destructive = false,
        Idempotent = true,
        UseStructuredContent = true
    )]
    [Description("Get book by id")]
    public async Task<GetBookOutput> Get(Guid id, CancellationToken cancellationToken)
    {
        var book = await _db.Books
            .Where(x => x.Id == id)
            .Select(x => new GetBookOutput.BookDto
            {
                Id = x.Id,
                Ean = x.Ean,
                Title = x.Title,
                Year = x.Year,
                AvailableCopies = x.AvailableCopies,
                Authors = x.Authors.Select(a => new GetBookOutput.AuthorDto
                {
                    Id = a.Id,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    BirthDate = a.BirthDate,
                    Biography = a.Biography,
                    Country = a.Country,
                    City = a.City,
                    PictureUrl = a.PictureUrl,
                })
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        return new GetBookOutput
        {
            Book = book,
        };
    }
}