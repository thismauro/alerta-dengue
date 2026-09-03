using System.Text.Json.Serialization;

namespace AlertaDengue.Application.DTOs;

public sealed record SemanaDisponivelDto
{
  
  [JsonPropertyName("ey")]
  public required int Ano { get; init; }

  [JsonPropertyName("ew")]
  public required int Semana { get; init; }

  [JsonPropertyName("semana_epidemiologica")]
  public required string SemanaEpidemiologica {get; init; }
}