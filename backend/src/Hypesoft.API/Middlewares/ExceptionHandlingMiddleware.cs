using System.Net;
using FluentValidation;
using Hypesoft.Application.Common.Exceptions;

namespace Hypesoft.API.Middlewares;

// Centraliza o tratamento de erro da API. Em vez de espalhar try/catch em cada
// controller/handler, qualquer excecao esperada (validacao, nao encontrado, etc)
// e traduzida aqui pra uma resposta HTTP com status e mensagem coerentes.
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await WriteResponse(context, HttpStatusCode.BadRequest, "Dados invalidos.", ex.Errors.Select(e => new
            {
                field = e.PropertyName,
                message = e.ErrorMessage
            }));
        }
        catch (NotFoundException ex)
        {
            await WriteResponse(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (CategoryInUseException ex)
        {
            await WriteResponse(context, HttpStatusCode.Conflict, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro nao tratado em {Path}", context.Request.Path);
            await WriteResponse(context, HttpStatusCode.InternalServerError, "Ocorreu um erro inesperado. Tente novamente em instantes.");
        }
    }

    private static async Task WriteResponse(HttpContext context, HttpStatusCode statusCode, string message, object? errors = null)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { message, errors });
    }
}
