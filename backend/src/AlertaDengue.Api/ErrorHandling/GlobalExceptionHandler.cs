using AlertaDengue.Application.Exceptions;
using AlertaDengue.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AlertaDengue.Api.ErrorHandling;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, titulo) = Mapear(exception);

        if (status >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Erro não tratado em {Metodo} {Caminho}",
                context.Request.Method, context.Request.Path);
        }
        else
        {
            _logger.LogWarning("Requisição recusada em {Caminho}: {Mensagem}",
                context.Request.Path, exception.Message);
        }

        context.Response.StatusCode = status;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = titulo,
                Detail = status >= StatusCodes.Status500InternalServerError
                    ? "Ocorreu um erro inesperado ao processar a requisição."
                    : exception.Message,
                Instance = $"{context.Request.Method} {context.Request.Path}"
            }
        });
    }

    private static (int Status, string Titulo) Mapear(Exception exception) => exception switch
    {
        AlertaNaoEncontradoException    => (StatusCodes.Status404NotFound,   "Registro não encontrado"),
        ApiExternaIndisponivelException => (StatusCodes.Status502BadGateway, "Serviço externo indisponível"),
        ArgumentOutOfRangeException     => (StatusCodes.Status400BadRequest, "Parâmetro fora do intervalo permitido"),
        ArgumentException               => (StatusCodes.Status400BadRequest, "Parâmetro inválido"),
        DomainException                 => (StatusCodes.Status400BadRequest, "Regra de negócio violada"),
        _                               => (StatusCodes.Status500InternalServerError, "Erro interno do servidor")
    };
}