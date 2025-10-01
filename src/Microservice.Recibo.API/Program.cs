using Microservice.Recibo.Application.Interfaces;
using Microservice.Recibo.Application.Services;
using Microservice.Recibo.Infrastructure.Data.Context;
using Microservice.Recibo.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microservice.Recibo.API.Data.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ReciboDbContext>(o => o.UseInMemoryDatabase("ReciboDb"));

builder.Services.AddAutoMapper(typeof(Microservice.Recibo.Application.Mapping.ReciboProfile).Assembly);

builder.Services.AddScoped<IReciboService, ReciboService>();
builder.Services.AddSingleton<IReciboRepository, ReciboRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<ReciboDbContext>();
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
