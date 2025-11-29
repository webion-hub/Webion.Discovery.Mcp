using System.ComponentModel;
using Library.Db;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;

namespace Library.Mcp.Public.Server.Tools.Authors.Get;

[McpServerToolType]
public sealed class GetAuthorTool
{
    private readonly LibraryDbContext _db;

    public GetAuthorTool(LibraryDbContext db)
    {
        _db = db;
    }
    
    
    [McpServerTool(
        Name = "get_author",
        Title = "Get author",
        Destructive = false,
        Idempotent = true,
        UseStructuredContent = true,
        OpenWorld = false
    )]
    [Description("Get author by ID")]
    public async Task<GetAuthorOutput> Borrow(Guid id, CancellationToken cancellationToken)
    {
        var author = await _db.Authors
            .Where(x => x.Id == id)
            .Select(x => new GetAuthorOutput.AuthorDto
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                BirthDate = x.BirthDate,
                Biography = x.Biography,
                Country = x.Country,
                City = x.City,
                PictureUrl = x.PictureUrl,
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        
        return new GetAuthorOutput
        {
            Author = author
        };
    }
}