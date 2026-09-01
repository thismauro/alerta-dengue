using AlertaDengue.Application.DTOs;
using AlertaDengue.Application.Interfaces;
using AlertaDengue.Domain.Interfaces;

namespace AlertaDengue.Application.Services;

public class AlertaService : IAlertaService
{
  public readonly IAlertaRepository _repository;

  public AlertaService(IAlertaRepository repository)
  {
    _repository = repository;
  }

  public async Task<DadosDengueResponseDto> BuscarPorSemanaAsync(int ano, int semana)
  {
    if (semana < 1 || semana > 53) throw new ArgumentException("Semana inválida! A semana deve ser entre 1 e 53.");
    if (ano < 2000 || ano > DateTime.Now.Year + 1) throw new ArgumentException("Ano inválido!");

    var alerta = await _repository.GetBySemanaAsync(ano, semana);

    if (alerta == null) return null;

    return new DadosDengueResponseDto
    {
      SemanaEpidemiologica = $"{alerta.Ano}-{alerta.Semana:D2}",
      CasosEstimados = alerta.CasosEstimados,
      CasosNotificados = alerta.CasosNotificados,
      NivelAlerta = alerta.NivelAlerta,
    };
  }
}