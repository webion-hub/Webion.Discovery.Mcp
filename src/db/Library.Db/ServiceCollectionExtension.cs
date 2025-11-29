using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Db;

public static class ServiceCollectionExtension
{
    public static void AddLibraryDb(this IServiceCollection services,
        string connectionString
    )
    {
        services.AddDbContextFactory<LibraryDbContext>(options =>
        {
            options.UseNpgsql(connectionString, b =>
            {
                b.MigrationsAssembly("Library.Db.Migrations");
            });
        });
    }
}