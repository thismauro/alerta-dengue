namespace AlertaDengue.Application.Interfaces;

public interface ISincronizacaoAlertaService
{
    Task<int> SincronizarAsync(CancellationToken cancellationToken);
}