using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AlertaDengue.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace AlertaDengue.Infrastructure.HttpClients;

public class AlertaDengueHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AlertaDengueHttpClient> _logger;

    public AlertaDengueHttpClient(HttpClient httpClient, ILogger<AlertaDengueHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<Alerta>> ConsultarDadosAsync(int ano, int semanaInicio, int semanaFim)
    {
        var geocode = "3106200";
        var disease = "dengue";
        var format = "json";

        var url = $"https://info.dengue.mat.br/api/alertcity?geocode={geocode}&disease={disease}&format={format}&ew_start={semanaInicio}&ew_end={semanaFim}&ey_start={ano}&ey_end={ano}";

        try
        {
            _logger.LogInformation("Consultando API AlertaDengue: {Url}", url);
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var dados = JsonSerializer.Deserialize<List<DadosApiExterna>>(json, options);

            if (dados == null || dados.Count == 0)
            {
                _logger.LogWarning("Nenhum dado retornado pela API");
                return new List<Alerta>();
            }

            var alertas = new List<Alerta>();
            foreach (var item in dados)
            {
                alertas.Add(new Alerta
                {
                    Ano = item.SE / 100,
                    Semana = item.SE % 100,
                    SemanaEpidemiologica = $"{item.SE / 100}-{item.SE % 100:D2}",
                    CasosEstimados = item.casos_est,
                    CasosNotificados = item.casos,
                    NivelAlerta = item.nivel,
                    DataRegistro = DateTime.Now
                });
            }

            _logger.LogInformation("Dados obtidos com sucesso: {Count} registros", alertas.Count);
            return alertas;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro de rede ao consultar API AlertaDengue");
            throw new Exception("Falha na comunicação com a API externa.", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Erro ao desserializar resposta da API");
            throw new Exception("Resposta da API em formato inválido.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao consultar API AlertaDengue");
            throw;
        }
    }

    private class DadosApiExterna
    {
        public int SE { get; set; }
        public int casos_est { get; set; }
        public int casos { get; set; }
        public int nivel { get; set; }
    }
}