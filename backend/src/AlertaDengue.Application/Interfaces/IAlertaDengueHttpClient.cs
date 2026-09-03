using AlertaDengue.Domain.Entities;
using AlertaDengue.Domain.ValueObjects;

namespace AlertaDengue.Application.Interfaces;

public interface IAlertaDengueHttpClient
{
    Task<IReadOnlyList<Alerta>> ConsultarAsync(
        SemanaEpidemiologica inicio,
        SemanaEpidemiologica fim,
        CancellationToken cancellationToken);
}