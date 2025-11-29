using System.ComponentModel;
using Library.Db;
using Library.Db.Entities.Library;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;
using Webion.Extensions.Linq;

namespace Library.Mcp.Public.Server.Tools.Books.Search;

[McpServerToolType]
public sealed class SearchBooksTool
{
    private readonly LibraryDbContext _db;

    public SearchBooksTool(LibraryDbContext db)
    {
        _db = db;
    }

    [McpServerTool(
        Name = "search_books",
        Title = "Search books",
        Destructive = false,
        Idempotent = true,
        UseStructuredContent = true
    )]
    [Description("Search books by title or author")]
    public async Task<SearchBookOutput> Search(
        string query,
        CancellationToken cancellationToken
    )
    {
        var books = await _db.Books
            .SearchInsensitive(query, tok => x => 
                x.Title.ToLower().Contains(tok) ||
                x.Authors.Any(a => 
                    a.FirstName.ToLower().Contains(tok) ||
                    a.LastName.ToLower().Contains(tok)
                )
            )
            .Select(x => new
            {
                x.Id,
                x.Title,
                x.Ean,
                x.Authors,
                x.Year,
                x.AvailableCopies,
                BorrowedCopies = x.BorrowRequests.Count(br => 
                    br.Status == BorrowStatus.Accepted
                ),
            })
            .OrderBy(x => x.AvailableCopies - x.BorrowedCopies)
            .Take(10)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        return new SearchBookOutput
        {
            Books = books.Select(x => new SearchBookOutput.BookDto
            {
                Id = x.Id,
                Ean = x.Ean,
                Title = x.Title,
                Authors = x.Authors.Select(a => new SearchBookOutput.AuthorDto
                {
                    Id = a.Id,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    BirthDate = a.BirthDate,
                    Biography = a.Biography,
                    Country = a.Country,
                    City = a.City,
                    PictureUrl = a.PictureUrl,
                }),
                Year = x.Year,
                AvailableCopies = x.AvailableCopies - x.BorrowedCopies,
            })
        };
    }
}