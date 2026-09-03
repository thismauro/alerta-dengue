namespace AlertaDengue.Domain.ValueObjects;

public readonly record struct SemanaEpidemiologica
{
    private const int AnoMinimo = 2000;

    public int Ano { get; }
    public int Numero { get; }

    public SemanaEpidemiologica(int ano, int numero)
    {
        if (ano < AnoMinimo || ano > DateTime.UtcNow.Year + 1)
            throw new ArgumentOutOfRangeException(nameof(ano),
                $"Ano deve estar entre {AnoMinimo} e {DateTime.UtcNow.Year + 1}.");

        if (numero < 1 || numero > TotalDeSemanasNoAno(ano))
            throw new ArgumentOutOfRangeException(nameof(numero),
                $"O ano {ano} possui {TotalDeSemanasNoAno(ano)} semanas epidemiológicas.");

        Ano = ano;
        Numero = numero;
    }

    public DateOnly DataInicial => PrimeiroDomingoDoAno(Ano).AddDays((Numero - 1) * 7);

    public DateOnly DataFinal => DataInicial.AddDays(6);

    public int Codigo => Ano * 100 + Numero;

    public static SemanaEpidemiologica De(DateOnly data)
    {
        var domingo = data.AddDays(-(int)data.DayOfWeek);
        var quarta = domingo.AddDays(3);
        var ano = quarta.Year;

        var numero = (domingo.DayNumber - PrimeiroDomingoDoAno(ano).DayNumber) / 7 + 1;
        return new SemanaEpidemiologica(ano, numero);
    }

    public static SemanaEpidemiologica DeCodigo(int codigo)
        => new(codigo / 100, codigo % 100);

    public SemanaEpidemiologica SubtrairSemanas(int quantidade)
        => De(DataInicial.AddDays(-7 * quantidade));

    public static SemanaEpidemiologica UltimaFechada(DateOnly hoje)
    {
        var atual = De(hoje);
        return hoje == atual.DataFinal ? atual : atual.SubtrairSemanas(1);
    }

    public IEnumerable<SemanaEpidemiologica> RetrocederAte(int quantidade)
    {
        for (var i = 0; i < quantidade; i++)
            yield return SubtrairSemanas(i);
    }

    private static DateOnly PrimeiroDomingoDoAno(int ano)
    {
        var quatroDeJaneiro = new DateOnly(ano, 1, 4);
        return quatroDeJaneiro.AddDays(-(int)quatroDeJaneiro.DayOfWeek);
    }

    private static int TotalDeSemanasNoAno(int ano)
        => (PrimeiroDomingoDoAno(ano + 1).DayNumber - PrimeiroDomingoDoAno(ano).DayNumber) / 7;

    public override string ToString() => $"{Ano}-{Numero:D2}";
}