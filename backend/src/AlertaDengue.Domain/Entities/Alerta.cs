namespace AlertaDengue.Domain.Entities;

public class Alerta {

  public int Id { get; set; }
  public int Ano { get; set; }
  public int Semana { get; set; }
  public string SemanaEpidemiologica { get; set; } = string.Empty;
  public int CasosEstimados { get; set; }
  public int CasosNotificados { get; set; }
  public int NivelAlerta { get; set; }
  public DateTime DataRegistro { get; set; }

  public bool SemanaValida() => Semana >= 1 && Semana <= 53;
  public bool NivelValido() => NivelAlerta >= 1 && NivelAlerta <= 3;
  public bool AnoValido() => Ano >= 2000 && Ano <= DateTime.Now.Year + 1;
}