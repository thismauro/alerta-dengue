using AlertaDengue.Domain.Entities;
using AlertaDengue.Domain.Enums;
using AlertaDengue.Domain.ValueObjects;
using Microsoft.Data.SqlClient;

namespace AlertaDengue.Infrastructure.Persistence;

internal static class AlertaMapper
{
    internal static Alerta Mapear(SqlDataReader reader) => Alerta.Reconstituir(
        id: reader.GetInt32(reader.GetOrdinal("id")),
        semana: new SemanaEpidemiologica(
            reader.GetInt32(reader.GetOrdinal("ano")),
            reader.GetInt32(reader.GetOrdinal("semana"))),
        casosEstimados: reader.GetDecimal(reader.GetOrdinal("casos_estimados")),
        casosNotificados: reader.GetInt32(reader.GetOrdinal("casos_notificados")),
        nivel: (NivelAlerta)reader.GetInt32(reader.GetOrdinal("nivel_alerta")),
        dataRegistroUtc: reader.GetDateTime(reader.GetOrdinal("data_registro_utc")));
}