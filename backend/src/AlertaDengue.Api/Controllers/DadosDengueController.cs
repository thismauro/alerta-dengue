using AlertaDengue.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AlertaDengue.Api.Controllers;

[ApiController]
[Route("api/dados-dengue")]
public class DadosDengueController : ControllerBase
{
  [HttpGet]
  [ProducesResponseType(typeof(DadosDengueResponseDto), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public IActionResult BuscarPorSemana([FromQuery] int ey, [FromQuery] int ew)
  {
    var response = new DadosDengueResponseDto
    {
      SemanaEpidemiologica = $"{ey}-{ew:D2}",
      CasosEstimados = 0,
      CasosNotificados = 0,
      NivelAlerta = 0
    };
    return Ok(response);
  }
}