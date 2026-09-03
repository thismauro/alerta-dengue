using AlertaDengue.Application.DTOs;
using AlertaDengue.Application.Interfaces;
using AlertaDengue.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace AlertaDengue.Api.Controllers;

[ApiController]
[Route("api/alertas")]
[Produces("application/json")]
public sealed class AlertasController : ControllerBase
{
    private readonly IConsultaAlertaService _consulta;
    private readonly ISincronizacaoAlertaService _sincronizacao;

    public AlertasController(
        IConsultaAlertaService consulta,
        ISincronizacaoAlertaService sincronizacao)
    {
        _consulta = consulta;
        _sincronizacao = sincronizacao;
    }

    [HttpGet]
    [ProducesResponseType<AlertaSemanalDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlertaSemanalDto>> ObterPorSemana(
        [FromQuery] int ey,
        [FromQuery] int ew,
        CancellationToken cancellationToken)
    {
        var semana = new SemanaEpidemiologica(ey, ew);
        var alerta = await _consulta.ObterPorSemanaAsync(semana, cancellationToken);

        return Ok(alerta);
    }

    [HttpPost("sincronizar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> Sincronizar(CancellationToken cancellationToken)
    {
        var registros = await _sincronizacao.SincronizarAsync(cancellationToken);

        return Ok(new { registros_sincronizados = registros });
    }

    [HttpGet("ultimas-semanas")]
    [ProducesResponseType<IReadOnlyList<SemanaDisponivelDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<IReadOnlyList<SemanaDisponivelDto>> ListarUltimaSemana(
        [FromQuery] int quantidade = 3)
        => Ok(_consulta.ListarUltimasSemanas(quantidade));
    
}