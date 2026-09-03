using AlertaDengue.Domain.ValueObjects;

namespace AlertaDengue.Domain.Exceptions;

public sealed class AlertaNaoEncontradoException : DomainException
{
  public SemanaEpidemiologica Semana { get; }

  public AlertaNaoEncontradoException(SemanaEpidemiologica semana) 
      : base($"Não há alerta registrado para a semana epidemiológica {semana}.")
  {
    Semana = semana;
  }
}