using Microservice.Estoque.Application.Interfaces;
using Microservice.Estoque.Application.Services;
using Microservice.Estoque.Infrastructure.Data.Context;
using Microservice.Estoque.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Banco de dados em memória
builder.Services.AddDbContext<EstoqueDbContext>(options =>
    options.UseInMemoryDatabase("EstoqueDb"));

builder.Services.AddAutoMapper(typeof(Microservice.Estoque.Application.Mapping.EstoqueProfile).Assembly);

builder.Services.AddScoped<IEstoqueRepository, EstoqueRepository>();
builder.Services.AddScoped<IEstoqueService, EstoqueService>();

var app = builder.Build();

// Garantir criação do "banco" em memória (opcional)
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<EstoqueDbContext>();
    ctx.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
