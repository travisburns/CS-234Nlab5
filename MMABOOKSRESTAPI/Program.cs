using MMABooksEFClasses.Models;
using Microsoft.EntityFrameworkCore;
using MMABooksEFClasses; // For ConfigDB

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add CORS policy - in a production app lock this down!
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(
        builder => {
            builder.AllowAnyOrigin()
                   .WithMethods("POST", "PUT", "DELETE", "GET", "OPTIONS")
                   .AllowAnyHeader();
        });
});

// Adding the DbContext to the service with proper MySQL configuration
builder.Services.AddDbContext<MMABooksContext>(options =>
{
    var connectionString = ConfigDB.GetMySqlConnectionString();
    var serverVersion = new MySqlServerVersion(new Version(8, 0));
    options.UseMySql(connectionString, serverVersion);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
// Don't forget to add UseCors before MapControllers!
app.UseCors();
app.MapControllers();

app.Run();