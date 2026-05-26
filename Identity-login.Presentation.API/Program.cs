using Identity.Application.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;


//Program.cs är hela applikationens startmotor och konfigurationsfil där hela API-projektet byggs upp
//när du startar det.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Identity.Infrastructure.Data.DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Tillåt frontenden (Shiko-sidan) att prata med detta API
//Denna inställning gör det enkelt under utvecklingen eftersom den tillåter anrop från alla lokala portar
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddScoped<ExternalApiClient>();

// Registrera dina Repositories och Services i Dependency Injection
builder.Services.AddScoped<Identity.Domain.Interface.IAuthRepository, Identity.Infrastructure.Repositories.AuthRepository>();
builder.Services.AddScoped<Identity.Application.Interfaces.IAuthService, Identity.Application.Services.AuthService>();

builder.Services.AddScoped<ExternalApiClient>();
builder.Services.AddHttpClient();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();