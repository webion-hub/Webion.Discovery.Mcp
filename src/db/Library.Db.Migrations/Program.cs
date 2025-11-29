using Library.Db;

var builder = WebApplication.CreateBuilder(args);
var conn = builder.Configuration.GetConnectionString("LibraryDb")!;

builder.Services.AddLibraryDb(conn);

var app = builder.Build();

app.Run();