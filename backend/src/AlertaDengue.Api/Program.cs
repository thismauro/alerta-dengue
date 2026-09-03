using System.Text.Json;
using AlertaDengue.Api.ErrorHandling;
using AlertaDengue.Application.Interfaces;
using AlertaDengue.Application.Services;
using AlertaDengue.Domain.Exceptions;
using AlertaDengue.Domain.ValueObjects;
using AlertaDengue.Infrastructure.Configuration;
using AlertaDengue.Infrastructure.HttpClients;
using AlertaDengue.Infrastructure.Persistence;
using AlertaDengue.Infrastructure.Repositories;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    });

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

const string PoliticaCors = "AllowAll";

builder.Services.AddCors(options =>
{
    options.AddPolicy(PoliticaCors, policy =>
        policy.WithOrigins(
                  "http://localhost:5500",
                  "http://127.0.0.1:5500",
                  "http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddScoped<IConsultaAlertaService, ConsultaAlertaService>();
builder.Services.AddScoped<ISincronizacaoAlertaService, SincronizacaoAlertaService>();

builder.Services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IAlertaRepository, AlertaRepository>();
builder.Services
    .AddOptions<AlertaDengueOptions>()
    .Bind(builder.Configuration.GetSection(AlertaDengueOptions.SecaoConfiguracao))
    .ValidateOnStart();

builder.Services
    .AddHttpClient<IAlertaDengueHttpClient, AlertaDengueHttpClient>((provedor, cliente) =>
    {
        var opcoes = provedor.GetRequiredService<IOptions<AlertaDengueOptions>>().Value;
        cliente.BaseAddress = new Uri(opcoes.BaseUrl);
        cliente.Timeout = TimeSpan.FromSeconds(opcoes.TimeoutSegundos);
    });

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors(PoliticaCors);

app.MapControllers();

app.Run();