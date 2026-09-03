using AlertaDengue.Application.DTOs;
using AlertaDengue.Application.Interfaces;
using AlertaDengue.Domain.Entities;
using AlertaDengue.Domain.Exceptions;
using AlertaDengue.Domain.ValueObjects;

namespace AlertaDengue.Application.Services;

public sealed class ConsultaAlertaService : IConsultaAlertaService
{
    private readonly IAlertaRepository _repository;

    public ConsultaAlertaService(IAlertaRepository repository)
        => _repository = repository;

    public async Task<AlertaSemanalDto> ObterPorSemanaAsync(
        SemanaEpidemiologica semana,
        CancellationToken cancellationToken)
    {
        var alerta = await _repository.ObterPorSemanaAsync(semana, cancellationToken)
            ?? throw new AlertaNaoEncontradoException(semana);

        return Mapear(alerta);
    }

    private static AlertaSemanalDto Mapear(Alerta alerta) => new()
    {
        SemanaEpidemiologica = alerta.Semana.ToString(),
        CasosEstimados = alerta.CasosEstimados,
        CasosNotificados = alerta.CasosNotificados,
        NivelAlerta = (int)alerta.Nivel
    };
}