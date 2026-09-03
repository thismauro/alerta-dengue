using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using AlertaDengue.Application.Exceptions;
using AlertaDengue.Application.Interfaces;
using AlertaDengue.Domain.Entities;
using AlertaDengue.Domain.Enums;
using AlertaDengue.Domain.ValueObjects;
using AlertaDengue.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlertaDengue.Infrastructure.HttpClients;

public sealed class AlertaDengueHttpClient : IAlertaDengueHttpClient
{
    private static readonly JsonSerializerOptions OpcoesJson = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    private readonly HttpClient _httpClient;
    private readonly AlertaDengueOptions _options;
    private readonly ILogger<AlertaDengueHttpClient> _logger;

    public AlertaDengueHttpClient(
        HttpClient httpClient,
        IOptions<AlertaDengueOptions> options,
        ILogger<AlertaDengueHttpClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Alerta>> ConsultarAsync(
        SemanaEpidemiologica inicio,
        SemanaEpidemiologica fim,
        CancellationToken cancellationToken)
    {
        var rota = MontarRota(inicio, fim);

        _logger.LogInformation(
            "Consultando alertas de {Inicio} a {Fim} para o município {Geocode}.",
            inicio, fim, _options.Geocode);

        try
        {
            using var resposta = await _httpClient.GetAsync(rota, cancellationToken);
            resposta.EnsureSuccessStatusCode();

            await using var conteudo = await resposta.Content.ReadAsStreamAsync(cancellationToken);

            var registros = await JsonSerializer.DeserializeAsync<List<AlertaDengueApiResponse>>(
                conteudo, OpcoesJson, cancellationToken);

            if (registros is null || registros.Count == 0)
            {
                _logger.LogWarning("A API não retornou registros para o intervalo consultado.");
                return [];
            }

            var alertas = registros.Select(Converter).ToList();

            _logger.LogInformation("{Quantidade} registros obtidos da API.", alertas.Count);
            return alertas;
        }
        catch (HttpRequestException excecao)
        {
            _logger.LogError(excecao, "Falha de comunicação com a API AlertaDengue.");
            throw new ApiExternaIndisponivelException(
                "Não foi possível comunicar com a API AlertaDengue.", excecao);
        }
        catch (TaskCanceledException excecao) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(excecao, "Tempo limite excedido ao consultar a API AlertaDengue.");
            throw new ApiExternaIndisponivelException(
                "A API AlertaDengue não respondeu dentro do tempo limite.", excecao);
        }
        catch (JsonException excecao)
        {
            _logger.LogError(excecao, "Resposta da API em formato inesperado.");
            throw new ApiExternaIndisponivelException(
                "A API AlertaDengue retornou dados em formato inesperado.", excecao);
        }
    }

    private string MontarRota(SemanaEpidemiologica inicio, SemanaEpidemiologica fim)
        => string.Create(CultureInfo.InvariantCulture,
            $"/api/alertcity?geocode={_options.Geocode}&disease={_options.Doenca}&format=json" +
            $"&ew_start={inicio.Numero}&ew_end={fim.Numero}" +
            $"&ey_start={inicio.Ano}&ey_end={fim.Ano}");

    private static Alerta Converter(AlertaDengueApiResponse registro) => new(
        semana: SemanaEpidemiologica.DeCodigo(registro.SE),
        casosEstimados: registro.CasosEstimados,
        casosNotificados: registro.Casos,
        nivel: (NivelAlerta)registro.Nivel);

    private sealed class AlertaDengueApiResponse
    {
        [JsonPropertyName("SE")]
        public int SE { get; init; }

        [JsonPropertyName("casos_est")]
        public decimal CasosEstimados { get; init; }

        [JsonPropertyName("casos")]
        public int Casos { get; init; }

        [JsonPropertyName("nivel")]
        public int Nivel { get; init; }
    }
}