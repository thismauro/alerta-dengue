const Api = (() => {
  const BASE_URL = "http://localhost:5248/api/alertas";

  class ApiError extends Error {
    constructor(mensagem, status) {
      super(mensagem);
      this.name = "ApiError";
      this.status = status;
    }
  }

  async function requisitar(rota) {
    let resposta;

    try {
      resposta = await fetch(`${BASE_URL}${rota}`);
    } catch {
      throw new ApiError(
        "Não foi possível conectar ao servidor. Verifique se a API está em execução.",
      );
    }

    if (!resposta.ok) {
      const problema = await resposta.json().catch(() => null);
      throw new ApiError(
        problema?.detail ?? `A requisição falhou (HTTP ${resposta.status}).`,
        resposta.status,
      );
    }

    return resposta.json();
  }

  return {
    ApiError,

    listarUltimasSemanas: (quantidade = 3) =>
      requisitar(`/ultimas-semanas?quantidade=${quantidade}`),

    obterPorSemana: (ey, ew) => requisitar(`?ey=${ey}&ew=${ew}`),
  };
})();
