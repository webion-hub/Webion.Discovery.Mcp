using Library.Db.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Db.Entities.Library;

public sealed class BorrowRequestDbo : IEntityTypeConfiguration<BorrowRequestDbo>
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public Guid UserId { get; set; }

    public BorrowStatus Status { get; set; } = BorrowStatus.Pending;
    
    public DateTimeOffset RequestedAt { get; set; }
    public DateTimeOffset? AcceptedAt { get; set; }
    public DateTimeOffset? ReturnedAt { get; set; }
    
    public BookDbo Book { get; set; } = null!;
    public UserDbo User { get; set; } = null!;
    
    public void Configure(EntityTypeBuilder<BorrowRequestDbo> builder)
    {
        builder.ToTable("borrow_request", Schemas.Library);
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Status).HasDefaultValue(BorrowStatus.Pending);
        
        builder
            .HasOne(x => x.Book)
            .WithMany(x => x.BorrowRequests)
            .HasForeignKey(x => x.BookId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasOne(x => x.User)
            .WithMany(x => x.BorrowRequests)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public enum BorrowStatus
{
    Pending,
    Accepted,
    Rejected,
    Returned,
}