using Library.Db.Entities.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Db.Entities.Identity;

public sealed class UserDbo : IEntityTypeConfiguration<UserDbo>
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    
    public List<BorrowRequestDbo> BorrowRequests { get; set; } = [];
    
    public void Configure(EntityTypeBuilder<UserDbo> builder)
    {
        builder.ToTable("user", Schemas.Identity);
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Email).IsUnique();
        
        builder.Property(x => x.Email).IsRequired().HasMaxLength(256);
        
        builder
            .HasMany(x => x.BorrowRequests)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}