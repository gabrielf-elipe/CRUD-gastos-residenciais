using Microsoft.EntityFrameworkCore;
using projetinho.data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // adicionando documentação do swagger
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

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
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = "swagger";
        // optei por usar o swagger, pois acho que é jeito mais convencional de visualizar e testar as requisições
    });
}

app.UseHttpsRedirection();

app.UseCors("PermitirReact"); // nao bloquear as requisições a api feitas pelo front

app.UseAuthorization();

app.MapControllers();

app.Run();
