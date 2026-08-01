using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GAMEHOSTING_APIREST.Database;
using GAMEHOSTING_APIREST.Entities;
using GAMEHOSTING_APIREST.Services;
using GAMEHOSTING_APIREST.Services.Interfaces;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Usa la misma base de datos que AppDbContext; maneja carrito, transacciones
// con Identity y usuarios (UserEntity).
builder.Services.AddDbContext<GameHostingDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<TransactionService>();
builder.Services.AddScoped<ICartService, CartService>();

// CORS para permitir peticiones desde el frontend React
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.MapControllers();

app.Run();