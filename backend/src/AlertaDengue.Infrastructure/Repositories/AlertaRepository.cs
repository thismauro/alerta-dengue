using System.Data;
using AlertaDengue.Application.Interfaces;
using AlertaDengue.Domain.Entities;
using AlertaDengue.Domain.ValueObjects;
using AlertaDengue.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;

namespace AlertaDengue.Infrastructure.Repositories;

public sealed class AlertaRepository : IAlertaRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public AlertaRepository(ISqlConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<Alerta?> ObterPorSemanaAsync(
        SemanaEpidemiologica semana,
        CancellationToken cancellationToken)
    {
        await using var conexao = await _connectionFactory.CriarConexaoAbertaAsync(cancellationToken);
        await using var comando = new SqlCommand(AlertaQueries.ObterPorSemana, conexao);

        comando.Parameters.Add("@ano", SqlDbType.Int).Value = semana.Ano;
        comando.Parameters.Add("@semana", SqlDbType.Int).Value = semana.Numero;

        await using var reader = await comando.ExecuteReaderAsync(cancellationToken);

        return await reader.ReadAsync(cancellationToken)
            ? AlertaMapper.Mapear(reader)
            : null;
    }

    public async Task<int> SalvarAsync(
        IEnumerable<Alerta> alertas,
        CancellationToken cancellationToken)
    {
        await using var conexao = await _connectionFactory.CriarConexaoAbertaAsync(cancellationToken);
        await using var transacao = (SqlTransaction)await conexao.BeginTransactionAsync(cancellationToken);

        try
        {
            var afetados = 0;

            foreach (var alerta in alertas)
            {
                await using var comando = new SqlCommand(AlertaQueries.Upsert, conexao, transacao);

                comando.Parameters.Add("@ano", SqlDbType.Int).Value = alerta.Semana.Ano;
                comando.Parameters.Add("@semana", SqlDbType.Int).Value = alerta.Semana.Numero;
                comando.Parameters.Add("@casosEstimados", SqlDbType.Decimal).Value = alerta.CasosEstimados;
                comando.Parameters.Add("@casosNotificados", SqlDbType.Int).Value = alerta.CasosNotificados;
                comando.Parameters.Add("@nivelAlerta", SqlDbType.Int).Value = (int)alerta.Nivel;
                comando.Parameters.Add("@dataRegistroUtc", SqlDbType.DateTime2).Value = alerta.DataRegistroUtc;

                afetados += await comando.ExecuteNonQueryAsync(cancellationToken);
            }

            await transacao.CommitAsync(cancellationToken);
            return afetados;
        }
        catch
        {
            await transacao.RollbackAsync(cancellationToken);
            throw;
        }
    }
}