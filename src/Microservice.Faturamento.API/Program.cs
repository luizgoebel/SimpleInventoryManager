using Microservice.Faturamento.API.Data.Seed;
using Microservice.Faturamento.Application.Clients;
using Microservice.Faturamento.Application.Interfaces;
using Microservice.Faturamento.Application.Services;
using Microservice.Faturamento.Infrastructure.Data.Context;
using Microservice.Faturamento.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Messaging;
using Shared.Messaging.InMemory;
using Microservice.Faturamento.Application.EventHandlers;
using Microservice.Pedido.Application.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(Microservice.Faturamento.Application.Mapping.FaturamentoProfile).Assembly);

builder.Services.AddDbContext<FaturaDbContext>(o => o.UseInMemoryDatabase("FaturaDb"));

builder.Services.AddScoped<IFaturamentoService, FaturamentoService>();
builder.Services.AddScoped<IFaturaRepository, FaturaRepository>();

builder.Services.AddHttpClient<IPedidoConsultaClient, PedidoConsultaClient>(c =>
{
    var url = builder.Configuration["ServiceUrls:PedidoAPI"]; if (!string.IsNullOrWhiteSpace(url)) c.BaseAddress = new Uri(url);
});
builder.Services.AddHttpClient<IReciboEmissaoClient, ReciboEmissaoClient>(c =>
{
    var url = builder.Configuration["ServiceUrls:ReciboAPI"]; if (!string.IsNullOrWhiteSpace(url)) c.BaseAddress = new Uri(url);
});

builder.Services.AddSingleton<IEventBus, InMemoryEventBus>();
// registrar handler
builder.Services.AddScoped<PedidoCriadoIntegrationEventHandler>();

var app = builder.Build();

// subscrever handler (após container pronto)
using (var scope = app.Services.CreateScope())
{
    var bus = scope.ServiceProvider.GetRequiredService<IEventBus>();
    bus.Subscribe<PedidoCriadoIntegrationEvent, PedidoCriadoIntegrationEventHandler>();

    var ctx = scope.ServiceProvider.GetRequiredService<FaturaDbContext>();
    ctx.Database.EnsureCreated();
    await ctx.SeedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
