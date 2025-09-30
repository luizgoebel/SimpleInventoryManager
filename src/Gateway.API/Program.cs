var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(opt =>
{
    opt.ListenAnyIP(5080);              // HTTP
    opt.ListenAnyIP(7082, o => o.UseHttps()); // HTTPS
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new
{
    Message = "Gateway API running",
    Routes = new[]
    {
        "/api/produtos/*",
        "/api/estoques/*",
        "/api/pedidos/*"
    }
}));

app.MapReverseProxy();

app.Run();