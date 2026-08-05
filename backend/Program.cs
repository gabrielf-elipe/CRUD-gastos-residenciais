using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using projetinho.data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddOpenApi();


var connectionString = builder.Configuration.GetConnectionString("appDbConnectionString");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirReact", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000") // portas pro frontend (em desenvolvimento)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{

    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("API Finanças Residenciais");
        options.DisableAgent();
        options.Layout = ScalarLayout.Classic; // mais semelhante ao do swagger :P
    });
}

app.UseHttpsRedirection();

app.UseCors("PermitirReact"); // nao bloquear as requisições a api feitas pelo front

app.UseAuthorization();

app.MapControllers();

app.Run();
