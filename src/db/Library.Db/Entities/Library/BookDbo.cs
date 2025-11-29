using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NpgsqlTypes;

namespace Library.Db.Entities.Library;

public sealed class BookDbo : IEntityTypeConfiguration<BookDbo>
{
    public Guid Id { get; set; }
    public string Ean { get; set; } = null!;
    
    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public int? Year { get; set; }
    public int AvailableCopies { get; set; }
    
    
    public List<AuthorDbo> Authors { get; set; } = [];
    public List<BorrowRequestDbo> BorrowRequests { get; set; } = [];


    public void Configure(EntityTypeBuilder<BookDbo> builder)
    {
        builder.ToTable("book", Schemas.Library);
        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => x.Ean);
        
        builder.Property(x => x.Title).IsRequired().HasMaxLength(256);
        builder.Property(x => x.Ean).IsRequired().HasMaxLength(13);
        builder.Property(x => x.Description).HasMaxLength(100_000);
    }
}