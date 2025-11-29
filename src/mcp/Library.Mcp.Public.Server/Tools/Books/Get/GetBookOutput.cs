namespace Library.Mcp.Public.Server.Tools.Books.Get;

public sealed class GetBookOutput
{
    public required BookDto? Book { get; init; }

    public sealed class BookDto
    {
        public required Guid Id { get; init; }
        public required string Ean { get; init; }
        public required string Title { get; init; }
        public required int? Year { get; init; }
        public required int AvailableCopies { get; init; }
        public required IEnumerable<AuthorDto> Authors { get; init; }
    }

    public sealed class AuthorDto
    {
        public required Guid Id { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required DateOnly? BirthDate { get; init; }
        public required string? Biography { get; init; }
        public required string? Country { get; init; }
        public required string? City { get; init; }
        public required string? PictureUrl { get; init; }
    }
}