namespace AlertaDengue.Infrastructure.Persistence;

internal static class AlertaQueries
{
  internal const string ObterPorSemana = """
      SELECT id, ano, semana, casos_estimados, casos_notificados, nivel_alerta, data_registro_utc
      FROM   alertas
      WHERE  ano = @ano AND semana = @semana;
      """;

  internal const string Upsert = """
      MERGE alertas WITH (HOLDLOCK) AS destino
      USING (SELECT @ano AS ano, @semana AS semana) AS origem
          ON destino.ano = origem.ano AND destino.semana = origem.semana
      WHEN MATCHED THEN
          UPDATE SET casos_estimados   = @casosEstimados,
                     casos_notificados = @casosNotificados,
                     nivel_alerta      = @nivelAlerta,
                     data_registro_utc = @dataRegistroUtc
      WHEN NOT MATCHED THEN
          INSERT (ano, semana, casos_estimados, casos_notificados, nivel_alerta, data_registro_utc)
          VALUES (@ano, @semana, @casosEstimados, @casosNotificados, @nivelAlerta, @dataRegistroUtc);
      """;
}