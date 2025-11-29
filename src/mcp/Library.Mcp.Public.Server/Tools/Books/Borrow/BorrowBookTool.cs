using System.ComponentModel;
using Library.Db;
using Library.Db.Entities.Identity;
using Library.Db.Entities.Library;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;

namespace Library.Mcp.Public.Server.Tools.Books.Borrow;

[McpServerToolType]
public sealed class BorrowBookTool
{
    private readonly LibraryDbContext _db;
    private readonly TimeProvider _timeProvider;

    public BorrowBookTool(LibraryDbContext db, TimeProvider timeProvider)
    {
        _db = db;
        _timeProvider = timeProvider;
    }
    
    
    [McpServerTool(
        Name = "borrow_book",
        Title = "Borrow book",
        Destructive = true,
        Idempotent = false,
        UseStructuredContent = true
    )]
    [Description("Borrow book by EAN")]
    public async Task<BorrowBookOutput> Borrow(
        string ean,
        string userEmail,
        CancellationToken cancellationToken
    )
    {
        var book = await _db.Books
            .Where(x => x.Ean == ean)
            .FirstOrDefaultAsync(cancellationToken);

        if (book is null)
        {
            return new BorrowBookOutput
            {
                Status = BorrowStatus.Rejected,
                Error = BorrowBookOutput.BorrowBookError.BookNotFound,
            };
        }
        
        await using var tsx = await _db.Database.BeginTransactionAsync(cancellationToken);
        var borrowedCopies = await _db.BorrowRequests
            .Where(x => x.BookId == book.Id)
            .Where(x => x.Status == BorrowStatus.Accepted)
            .CountAsync(cancellationToken);

        var availableCopies = book.AvailableCopies - borrowedCopies;
        if (availableCopies <= 0)
        {
            return new BorrowBookOutput
            {
                Status = BorrowStatus.Rejected,
                Error = BorrowBookOutput.BorrowBookError.BookNotAvailable,
            };
        }
        
        var user = await GetUserAsync(userEmail, cancellationToken);
        var now = _timeProvider.GetUtcNow();
        var borrow = new BorrowRequestDbo
        {
            Id = Guid.NewGuid(),
            BookId = book.Id,
            UserId = user.Id,
            Status = BorrowStatus.Accepted,
            RequestedAt = now,
            AcceptedAt = now,
        };

        _db.BorrowRequests.Add(borrow);
        await _db.SaveChangesAsync(cancellationToken);
        await tsx.CommitAsync(cancellationToken);
        
        return new BorrowBookOutput
        {
            Status = BorrowStatus.Accepted,
            Error = null,
        };
    }

    private async Task<UserDbo> GetUserAsync(string userEmail, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .Where(x => x.Email == userEmail)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is not null)
            return user;
        
        var newUser = new UserDbo
        {
            Id = Guid.NewGuid(),
            Email = userEmail,
        };
        
        _db.Users.Add(newUser);
        await _db.SaveChangesAsync(cancellationToken);
        return newUser;
    }
}