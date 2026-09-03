namespace AlertaDengue.Domain.Exceptions;

public abstract class DomainException : Exception
{
  protected DomainException(string mensagem) : base(mensagem) {}

  protected DomainException(string mensagem, Exception innerException) 
      : base(mensagem, innerException) {}
}