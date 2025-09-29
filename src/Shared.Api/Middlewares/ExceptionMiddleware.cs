using Microsoft.AspNetCore.Http;
using Shared.Application.Exceptions;
using Shared.Domain.Entities;

namespace BookCatalog.Api.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private const int ExceptionStatusCode = 500;
    private const int ServiceExceptionStatusCode = 503;
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (ServiceException ex)
        {
            await HandleExceptionAsync(httpContext, ex, ServiceExceptionStatusCode, ex.ObjetoErro);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex, ExceptionStatusCode);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        int statusCode,
        object objetoErro = null)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(new DetalhesDoErro()
        {
            StatusCode = context.Response.StatusCode,
            Mensagem = exception.Message,
            ObjetoErro = objetoErro
        }.ToString());
    }
}