using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace AlertaDengue.Infrastructure.Persistence;

public sealed class SqlConnectionFactory : ISqlConnectionFactory
{
  private const string NomeDaConexao = "DefaultConnection";
  private readonly string _connectionString;

  public SqlConnectionFactory(IConfiguration configuration)
  {
    _connectionString = configuration.GetConnectionString(NomeDaConexao)
      ?? throw new InvalidOperationException($"A string de conexão '{NomeDaConexao}' não foi encontrada na configuração.");
  }

  public async Task<SqlConnection> CriarConexaoAbertaAsync(CancellationToken cancellationToken)
  {
    var conexao = new SqlConnection(_connectionString);
    await conexao.OpenAsync(cancellationToken);
    return conexao;
  }
}