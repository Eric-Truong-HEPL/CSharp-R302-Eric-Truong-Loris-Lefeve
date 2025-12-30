using KindomHospital.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
// Apply migrations and seed database
try
{
    SeedData.Initialize(app.Services);
}
catch (Exception ex)
{
    // If seeding fails at startup, log to console. Local fix recommended.
    Console.WriteLine($"Database seed failed: {ex.Message}");
}
//Ajouter les mappers au di
//Ajouter les services au di
//Ajouter les repositories au di
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
