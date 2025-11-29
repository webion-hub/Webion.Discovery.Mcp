namespace Library.Mcp.Public.Server.Tools.Authors.Get;

public sealed class GetAuthorOutput
{
    public required AuthorDto? Author { get; init; }
    
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