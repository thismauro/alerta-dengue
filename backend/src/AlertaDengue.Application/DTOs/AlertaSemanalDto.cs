using System.Text.Json.Serialization;

namespace AlertaDengue.Application.DTOs;

public sealed record AlertaSemanalDto
{
    [JsonPropertyName("semana_epidemiologica")]
    public required string SemanaEpidemiologica { get; init; }

    [JsonPropertyName("casos_est")]
    public required decimal CasosEstimados { get; init; }

    [JsonPropertyName("casos_notificados")]
    public required int CasosNotificados { get; init; }

    [JsonPropertyName("nivel_alerta")]
    public required int NivelAlerta { get; init; }
}