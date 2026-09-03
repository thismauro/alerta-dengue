using Microsoft.Data.SqlClient;

namespace AlertaDengue.Infrastructure.Persistence;

public interface ISqlConnectionFactory
{
  Task<SqlConnection> CriarConexaoAbertaAsync(CancellationToken cancellationToken);
}