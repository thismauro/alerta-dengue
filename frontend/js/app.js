(() => {
  const QUANTIDADE_SEMANAS = 3;

  const NIVEIS = {
    1: {
      nome: "Verde",
      cor: "var(--nivel-1)",
      descricao: "Situação controlada",
    },
    2: {
      nome: "Amarelo",
      cor: "var(--nivel-2)",
      descricao: "Alerta",
    },
    3: {
      nome: "Laranja",
      cor: "var(--nivel-3)",
      descricao: "Transmissão sustentada",
    },
    4: {
      nome: "Vermelho",
      cor: "var(--nivel-4)",
      descricao: "Nível epidêmico",
    },
  };

  const elementos = {
    cards: document.getElementById("cards"),
    carregando: document.getElementById("estado-carregando"),
    erro: document.getElementById("estado-erro"),
    mensagemErro: document.getElementById("mensagem-erro"),
    recarregar: document.getElementById("botao-recarregar"),
  };

  const formatarNumero = (valor) =>
    new Intl.NumberFormat("pt-BR", { maximumFractionDigits: 1 }).format(valor);

  function descreverPeriodo(ano, semana) {
    const quatroDeJaneiro = new Date(Date.UTC(ano, 0, 4));
    const primeiroDomingo = new Date(quatroDeJaneiro);
    primeiroDomingo.setUTCDate(
      quatroDeJaneiro.getUTCDate() - quatroDeJaneiro.getUTCDay(),
    );

    const inicio = new Date(primeiroDomingo);
    inicio.setUTCDate(primeiroDomingo.getUTCDate() + (semana - 1) * 7);

    const fim = new Date(inicio);
    fim.setUTCDate(inicio.getUTCDate() + 6);

    const formatar = (data) =>
      data.toLocaleDateString("pt-BR", {
        day: "2-digit",
        month: "2-digit",
        timeZone: "UTC",
      });

    return `${formatar(inicio)} a ${formatar(fim)}`;
  }

  function criarCard(semana, alerta) {
    const nivel = NIVEIS[alerta?.nivel_alerta] ?? null;

    const artigo = document.createElement("article");
    artigo.className = alerta ? "card" : "card card--indisponivel";
    if (nivel) artigo.style.setProperty("--cor-nivel", nivel.cor);

    const cabecalho = document.createElement("div");
    cabecalho.className = "card__cabecalho";
    cabecalho.innerHTML = `
            <h2 class="card__semana">Semana ${semana.semana_epidemiologica}</h2>
            <p class="card__periodo">${descreverPeriodo(semana.ey, semana.ew)}</p>
        `;

    const corpo = document.createElement("div");
    corpo.className = "card__corpo";

    if (!alerta) {
      corpo.innerHTML =
        '<p class="metrica__rotulo">Dados indisponíveis para esta semana.</p>';
    } else {
      corpo.innerHTML = `
                <div class="metrica">
                    <span class="metrica__rotulo">Casos estimados</span>
                    <span class="metrica__valor">${formatarNumero(alerta.casos_est)}</span>
                </div>
                <div class="metrica">
                    <span class="metrica__rotulo">Casos notificados</span>
                    <span class="metrica__valor">${formatarNumero(alerta.casos_notificados)}</span>
                </div>
                <span class="selo">Nível ${alerta.nivel_alerta} &middot; ${nivel?.nome ?? "Desconhecido"}</span>
            `;
    }

    artigo.append(cabecalho, corpo);
    return artigo;
  }

  function exibirErro(mensagem) {
    elementos.mensagemErro.textContent = mensagem;
    elementos.erro.hidden = false;
  }

  async function carregar() {
    alternar(elementos.carregando, true);
    alternar(elementos.erro, false);
    elementos.cards.innerHTML = "";

    try {
      const semanas = await Api.listarUltimasSemanas(QUANTIDADE_SEMANAS);

      const resultados = await Promise.all(
        semanas.map(async (semana) => {
          try {
            return {
              semana,
              alerta: await Api.obterPorSemana(semana.ey, semana.ew),
            };
          } catch (erro) {
            if (erro instanceof Api.ApiError && erro.status === 404) {
              return { semana, alerta: null };
            }
            throw erro;
          }
        }),
      );

      resultados.forEach(({ semana, alerta }) =>
        elementos.cards.append(criarCard(semana, alerta)),
      );
    } catch (erro) {
      console.error("Falha ao carregar alertas:", erro);
      exibirErro(erro.message || "Erro inesperado ao consultar a API.");
    } finally {
      alternar(elementos.carregando, false);
    }
  }

  function alternar(elemento, visivel) {
    if (elemento) elemento.hidden = !visivel;
  }

  elementos.recarregar.addEventListener("click", carregar);
  carregar();
})();
