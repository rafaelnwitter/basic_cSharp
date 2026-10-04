namespace DesafioComercial.Juros;

public sealed record ResultadoJuros(
    decimal ValorOriginal,
    DateOnly Vencimento,
    DateOnly DataReferencia,
    int DiasEmAtraso,
    decimal AliquotaDiaria,
    decimal Juros,
    decimal ValorAtualizado);
