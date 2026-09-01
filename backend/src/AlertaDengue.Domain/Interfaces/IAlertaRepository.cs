using AlertaDengue.Domain.Entities;

namespace AlertaDengue.Domain.Interfaces;

public interface IAlertaRepository
{
  Task<Alerta> GetBySemanaAsync(int ano, int semana);
  Task<int> UpsertAsync(Alerta alerta);
}