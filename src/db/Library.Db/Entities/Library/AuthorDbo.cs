using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Db.Entities.Library;

public sealed class AuthorDbo : IEntityTypeConfiguration<AuthorDbo>
{
    public Guid Id { get; set; }
    
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateOnly? BirthDate { get; set; }
    public string? Biography { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? PictureUrl { get; set; }
    
    public List<BookDbo> Books { get; set; } = [];
    
    public void Configure(EntityTypeBuilder<AuthorDbo> builder)
    {
        builder.ToTable("author", Schemas.Library);
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(256);
        builder.Property(x => x.LastName).IsRequired().HasMaxLength(256);
        builder.Property(x => x.Biography).HasMaxLength(100_000);
        builder.Property(x => x.PictureUrl).HasMaxLength(256);
        builder.Property(x => x.Country).HasMaxLength(128);
        builder.Property(x => x.City).HasMaxLength(128);
        
        builder
            .HasMany(x => x.Books)
            .WithMany(x => x.Authors)
            .UsingEntity<BookAuthorDbo>(
                j => j.HasOne(x => x.Book).WithMany(),
                j => j.HasOne(x => x.Author).WithMany()
            );
    }
}