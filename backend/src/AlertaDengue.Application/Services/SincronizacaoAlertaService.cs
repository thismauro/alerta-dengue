using AlertaDengue.Application.Interfaces;
using AlertaDengue.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace AlertaDengue.Application.Services;

public sealed class SincronizacaoAlertaService : ISincronizacaoAlertaService
{
    private const int SemanasRetroativas = 26;

    private readonly IAlertaDengueHttpClient _httpClient;
    private readonly IAlertaRepository _repository;
    private readonly ILogger<SincronizacaoAlertaService> _logger;

    public SincronizacaoAlertaService(
        IAlertaDengueHttpClient httpClient,
        IAlertaRepository repository,
        ILogger<SincronizacaoAlertaService> logger)
    {
        _httpClient = httpClient;
        _repository = repository;
        _logger = logger;
    }

    public async Task<int> SincronizarAsync(CancellationToken cancellationToken)
    {
        var fim = SemanaEpidemiologica.UltimaFechada(DateOnly.FromDateTime(DateTime.UtcNow.Date));
        var inicio = fim.SubtrairSemanas(SemanasRetroativas - 1);

        _logger.LogInformation("Iniciando sincronização de {Inicio} a {Fim}.", inicio, fim);

        var alertas = await _httpClient.ConsultarAsync(inicio, fim, cancellationToken);

        if (alertas.Count == 0)
        {
            _logger.LogWarning("Nenhum alerta retornado pela API para o período.");
            return 0;
        }

        var gravados = await _repository.SalvarAsync(alertas, cancellationToken);

        _logger.LogInformation("Sincronização concluída: {Gravados} registros.", gravados);
        return gravados;
    }
}