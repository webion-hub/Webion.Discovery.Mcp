using Library.Db.Entities.Library;

namespace Library.Mcp.Public.Server.Tools.Books.Borrow;

public sealed class BorrowBookOutput
{
    public required BorrowStatus Status { get; init; }
    public required BorrowBookError? Error { get; init; }
    
    public enum BorrowBookError
    {
        BookNotFound,
        BookNotAvailable,
    }
}

