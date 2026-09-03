namespace AlertaDengue.Infrastructure.Configuration;

public sealed class AlertaDengueOptions
{
    public const string SecaoConfiguracao = "AlertaDengue";

    public string BaseUrl { get; init; } = string.Empty;

    public string Geocode { get; init; } = string.Empty;

    public string Doenca { get; init; } = "dengue";

    public int TimeoutSegundos { get; init; } = 30;

    public int SemanasRetroativas { get; init; } = 26;
}