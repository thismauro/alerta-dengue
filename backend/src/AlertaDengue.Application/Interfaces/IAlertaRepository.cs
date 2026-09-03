using AlertaDengue.Domain.Entities;
using AlertaDengue.Domain.ValueObjects;

namespace AlertaDengue.Application.Interfaces;

public interface IAlertaRepository
{
  Task<Alerta?> ObterPorSemanaAsync(SemanaEpidemiologica semana, CancellationToken cancellationToken);

  Task<int> SalvarAsync(IEnumerable<Alerta> alertas, CancellationToken cancellationToken);
}