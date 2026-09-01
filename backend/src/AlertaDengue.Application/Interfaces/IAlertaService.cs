using AlertaDengue.Application.DTOs;

namespace AlertaDengue.Application.Interfaces;

public interface IAlertaService { 
    
  Task<DadosDengueResponseDto> BuscarPorSemanaAsync(int ano, int semana);
}