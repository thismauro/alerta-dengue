namespace AlertaDengue.Application.DTOs;

public class DadosDengueResponseDto
{
  public string SemanaEpidemiologica { get; set; } = string.Empty;
  public int CasosEstimados { get; set;}
  public int CasosNotificados { get; set; }
  public int NivelAlerta { get; set; }
}