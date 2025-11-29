using Library.Db.Entities.Identity;
using Library.Db.Entities.Library;
using Microsoft.EntityFrameworkCore;

namespace Library.Db;

public sealed class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
    {
    }

    // Library
    public DbSet<BookDbo> Books { get; set; } = null!;
    public DbSet<AuthorDbo> Authors { get; set; } = null!;
    public DbSet<BorrowRequestDbo> BorrowRequests { get; set; } = null!;
    
    // Identity
    public DbSet<UserDbo> Users { get; set; } = null!;
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibraryDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        base.ConfigureConventions(builder);
        builder.Properties<Enum>().HaveConversion<string>();
    }
}