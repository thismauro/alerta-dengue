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

    public IReadOnlyList<SemanaDisponivelDto> ListarUltimasSemanas(int quantidade)
{
    if (quantidade is < 1 or > 52)
        throw new ArgumentOutOfRangeException(nameof(quantidade),
            "A quantidade de semanas deve estar entre 1 e 52.");

    var referencia = SemanaEpidemiologica.UltimaFechada(DateOnly.FromDateTime(DateTime.UtcNow.Date));

    return referencia.RetrocederAte(quantidade)
        .Select(semana => new SemanaDisponivelDto
        {
            Ano = semana.Ano,
            Semana = semana.Numero,
            SemanaEpidemiologica = semana.ToString()
        })
        .ToList();
    }
}