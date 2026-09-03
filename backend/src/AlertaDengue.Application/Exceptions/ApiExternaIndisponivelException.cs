using AlertaDengue.Domain.Exceptions;

namespace AlertaDengue.Application.Exceptions;

public sealed class ApiExternaIndisponivelException : Exception
{
    public ApiExternaIndisponivelException(string mensagem, Exception? innerException = null)
        : base(mensagem, innerException) {}
}