using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Db.Entities.Library;

public sealed class BookAuthorDbo : IEntityTypeConfiguration<BookAuthorDbo>
{
    public Guid BookId { get; set; }
    public Guid AuthorId { get; set; }
    
    public BookDbo Book { get; set; } = null!;
    public AuthorDbo Author { get; set; } = null!;
    
    public void Configure(EntityTypeBuilder<BookAuthorDbo> builder)
    {
        builder.ToTable("book_author", Schemas.Library);
        builder.HasKey(x => new {x.BookId, x.AuthorId});
        
        builder
            .HasOne(x => x.Book)
            .WithMany()
            .HasForeignKey(x => x.BookId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasOne(x => x.Author)
            .WithMany()
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}