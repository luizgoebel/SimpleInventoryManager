using Microservice.Pedido.Application.Clients;
using Microservice.Pedido.Application.Interfaces;
using Microservice.Pedido.Application.Mapping;
using Microservice.Pedido.Application.Services;
using Microservice.Pedido.Infrastructure.Data.Context;
using Microservice.Pedido.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PedidoDbContext>(o => o.UseInMemoryDatabase("PedidoDb"));

builder.Services.AddAutoMapper(typeof(PedidoProfile).Assembly);

builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IPedidoService, PedidoService>();

builder.Services.AddHttpClient<IEstoqueMovimentoClient, EstoqueMovimentoClient>(c =>
{
    var baseUrl = builder.Configuration["ServiceUrls:EstoqueAPI"];
    if (!string.IsNullOrWhiteSpace(baseUrl))
        c.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<PedidoDbContext>();
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
