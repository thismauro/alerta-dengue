using AlertaDengue.Domain.Entities;
using AlertaDengue.Domain.Interfaces;
using AlertaDengue.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace AlertaDengue.Infrastructure.Repositories
{
    public class AlertaRepository : IAlertaRepository
    {
        private readonly Database _database;

        public AlertaRepository(Database database)
        {
            _database = database;
        }

        public async Task<Alerta> GetBySemanaAsync(int ano, int semana)
        {
            using var connection = _database.GetConnection();
            var command = new SqlCommand(
                "SELECT * FROM alertas WHERE ano = @ano AND semana = @semana",
                connection
            );
            command.Parameters.AddWithValue("@ano", ano);
            command.Parameters.AddWithValue("@semana", semana);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Alerta
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Ano = reader.GetInt32(reader.GetOrdinal("ano")),
                    Semana = reader.GetInt32(reader.GetOrdinal("semana")),
                    SemanaEpidemiologica = reader.GetString(reader.GetOrdinal("semana_epidemiologica")),
                    CasosEstimados = reader.GetInt32(reader.GetOrdinal("casos_estimados")),
                    CasosNotificados = reader.GetInt32(reader.GetOrdinal("casos_notificados")),
                    NivelAlerta = reader.GetInt32(reader.GetOrdinal("nivel_alerta")),
                    DataRegistro = reader.GetDateTime(reader.GetOrdinal("data_registro"))
                };
            }

            return null;
        }

        public async Task<int> UpsertAsync(Alerta alerta)
        {
            using var connection = _database.GetConnection();

            var exists = await ExistsAsync(alerta.Ano, alerta.Semana);

            if (exists)
            {
                var command = new SqlCommand(
                    @"UPDATE alertas SET 
                        casos_estimados = @casosEstimados,
                        casos_notificados = @casosNotificados,
                        nivel_alerta = @nivelAlerta
                      WHERE ano = @ano AND semana = @semana",
                    connection
                );
                command.Parameters.AddWithValue("@casosEstimados", alerta.CasosEstimados);
                command.Parameters.AddWithValue("@casosNotificados", alerta.CasosNotificados);
                command.Parameters.AddWithValue("@nivelAlerta", alerta.NivelAlerta);
                command.Parameters.AddWithValue("@ano", alerta.Ano);
                command.Parameters.AddWithValue("@semana", alerta.Semana);

                await connection.OpenAsync();
                return await command.ExecuteNonQueryAsync();
            }
            else
            {
                var command = new SqlCommand(
                    @"INSERT INTO alertas 
                        (ano, semana, semana_epidemiologica, casos_estimados, casos_notificados, nivel_alerta, data_registro) 
                      VALUES 
                        (@ano, @semana, @semanaEpidemiologica, @casosEstimados, @casosNotificados, @nivelAlerta, @dataRegistro)",
                    connection
                );
                command.Parameters.AddWithValue("@ano", alerta.Ano);
                command.Parameters.AddWithValue("@semana", alerta.Semana);
                command.Parameters.AddWithValue("@semanaEpidemiologica", alerta.SemanaEpidemiologica);
                command.Parameters.AddWithValue("@casosEstimados", alerta.CasosEstimados);
                command.Parameters.AddWithValue("@casosNotificados", alerta.CasosNotificados);
                command.Parameters.AddWithValue("@nivelAlerta", alerta.NivelAlerta);
                command.Parameters.AddWithValue("@dataRegistro", alerta.DataRegistro);

                await connection.OpenAsync();
                return await command.ExecuteNonQueryAsync();
            }
        }

        private async Task<bool> ExistsAsync(int ano, int semana)
        {
            using var connection = _database.GetConnection();
            var command = new SqlCommand(
                "SELECT COUNT(1) FROM alertas WHERE ano = @ano AND semana = @semana",
                connection
            );
            command.Parameters.AddWithValue("@ano", ano);
            command.Parameters.AddWithValue("@semana", semana);

            await connection.OpenAsync();
            var count = Convert.ToInt32(await command.ExecuteScalarAsync());
            return count > 0;
        }
    }
}