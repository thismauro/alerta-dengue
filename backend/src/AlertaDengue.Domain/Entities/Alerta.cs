using AlertaDengue.Domain.Enums;
using AlertaDengue.Domain.ValueObjects;

namespace AlertaDengue.Domain.Entities;

public class Alerta
{
    public int Id { get; private set; }
    public SemanaEpidemiologica Semana { get; private set; }

    public decimal CasosEstimados { get; private set; }

    public int CasosNotificados { get; private set; }

    public NivelAlerta Nivel { get; private set; }
    public DateTime DataRegistroUtc { get; private set; }

    public Alerta(
        SemanaEpidemiologica semana,
        decimal casosEstimados,
        int casosNotificados,
        NivelAlerta nivel)
    {
        if (casosEstimados < 0)
            throw new ArgumentOutOfRangeException(nameof(casosEstimados),
                "Os números de casos estimados não podem ser negativos.");

        if (casosNotificados < 0)
            throw new ArgumentOutOfRangeException(nameof(casosNotificados),
                "Os números de casos notificados não podem ser negativos.");

        if (!Enum.IsDefined(nivel))
            throw new ArgumentOutOfRangeException(nameof(nivel),
                $"O nível de alerta informado não foi encontrado: {(int)nivel}");

        Semana = semana;
        CasosEstimados = casosEstimados;
        CasosNotificados = casosNotificados;
        Nivel = nivel;
        DataRegistroUtc = DateTime.UtcNow;
    }

    public static Alerta Reconstituir(
        int id,
        SemanaEpidemiologica semana,
        decimal casosEstimados,
        int casosNotificados,
        NivelAlerta nivel,
        DateTime dataRegistroUtc)
        => new(semana, casosEstimados, casosNotificados, nivel)
        {
            Id = id,
            DataRegistroUtc = dataRegistroUtc
        };
}