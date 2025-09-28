using Microservice.Produto.Application.Interfaces;
using Microservice.Produto.Application.Services;
using Microservice.Produto.Infrastructure.Data.Context;
using Microservice.Produto.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configura��o do DbContext para usar MySQL ou InMemory conforme configura��o
builder.Services.AddDbContext<ProdutoDbContext>(options =>
{
    var useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");
    if (useInMemory)
    {
        options.UseInMemoryDatabase("ProdutoDb");
    }
    else
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' n�o configurada.");

        var serverVersion = ServerVersion.AutoDetect(connectionString);
        options.UseMySql(connectionString, serverVersion);
    }
});

builder.Services.AddAutoMapper(typeof(Microservice.Produto.Application.Mapping.ProdutoProfile).Assembly);

builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();

builder.Services.AddHttpClient<IEstoqueClient, EstoqueClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:EstoqueAPI"]!);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
