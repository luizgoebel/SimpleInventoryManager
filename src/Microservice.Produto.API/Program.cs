using Microservice.Produto.Application.Interfaces;
using Microservice.Produto.Application.Services;
using Microservice.Produto.Infrastructure.Data.Context;
using Microservice.Produto.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ProdutoDbContext>(o => o.UseInMemoryDatabase("ProdutoDb"));

builder.Services.AddAutoMapper(typeof(Microservice.Produto.Application.Mapping.ProdutoProfile).Assembly);

builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();

builder.Services.AddHttpClient<IEstoqueClient, EstoqueClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:EstoqueAPI"]!);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<ProdutoDbContext>();
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
