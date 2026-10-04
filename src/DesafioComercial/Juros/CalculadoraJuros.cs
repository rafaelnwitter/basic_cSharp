namespace DesafioComercial.Juros;

/// <summary>
/// Juros simples de 2,5% ao dia sobre o valor original.
/// O atraso começa no dia seguinte ao vencimento. Na data de vencimento, juros = 0.
/// </summary>
public static class CalculadoraJuros
{
    public const decimal AliquotaDiaria = 0.025m;

    public static ResultadoJuros Calcular(decimal valor, DateOnly vencimento, DateOnly dataReferencia)
    {
        if (valor < 0)
            throw new ArgumentOutOfRangeException(nameof(valor), "O valor não pode ser negativo.");

        var dias = dataReferencia.DayNumber - vencimento.DayNumber;
        var diasEmAtraso = Math.Max(0, dias);
        var juros = decimal.Round(valor * AliquotaDiaria * diasEmAtraso, 2, MidpointRounding.AwayFromZero);

        return new ResultadoJuros(
            valor,
            vencimento,
            dataReferencia,
            diasEmAtraso,
            AliquotaDiaria,
            juros,
            valor + juros);
    }
}
