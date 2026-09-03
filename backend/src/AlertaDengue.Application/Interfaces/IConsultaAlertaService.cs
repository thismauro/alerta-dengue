using AlertaDengue.Application.DTOs;
using AlertaDengue.Domain.ValueObjects;

namespace AlertaDengue.Application.Interfaces;

public interface IConsultaAlertaService
{
    Task<AlertaSemanalDto> ObterPorSemanaAsync(
        SemanaEpidemiologica semana,
        CancellationToken cancellationToken);

    IReadOnlyList<SemanaDisponivelDto> ListarUltimasSemanas(int quantidade);
}