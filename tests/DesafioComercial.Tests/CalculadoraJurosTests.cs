using Xunit;
using DesafioComercial.Juros;

namespace DesafioComercial.Tests;

public class CalculadoraJurosTests
{
    private static readonly DateOnly Hoje = new(2026, 10, 3);

    [Fact]
    public void Sem_atraso_nao_gera_juros()
    {
        var noVencimento = CalculadoraJuros.Calcular(1000m, Hoje, Hoje);
        var aVencer = CalculadoraJuros.Calcular(1000m, Hoje.AddDays(2), Hoje);

        Assert.Equal(0, noVencimento.DiasEmAtraso);
        Assert.Equal(0m, noVencimento.Juros);
        Assert.Equal(1000m, noVencimento.ValorAtualizado);
        Assert.Equal(0m, aVencer.Juros);
    }

    [Fact]
    public void Aplica_dois_e_meio_por_cento_simples_por_dia_corrido()
    {
        var resultado = CalculadoraJuros.Calcular(1000m, new DateOnly(2026, 10, 1), Hoje);

        Assert.Equal(2, resultado.DiasEmAtraso);
        Assert.Equal(0.025m, resultado.AliquotaDiaria);
        Assert.Equal(50.00m, resultado.Juros);
        Assert.Equal(1050.00m, resultado.ValorAtualizado);
    }

    [Fact]
    public void Arredonda_o_juros_para_duas_casas()
    {
        var resultado = CalculadoraJuros.Calcular(1000.33m, Hoje.AddDays(-3), Hoje);

        Assert.Equal(75.02m, resultado.Juros);
        Assert.Equal(1075.35m, resultado.ValorAtualizado);
    }

    [Fact]
    public void Trinta_dias_de_atraso()
    {
        var resultado = CalculadoraJuros.Calcular(800m, new DateOnly(2026, 9, 3), Hoje);

        Assert.Equal(30, resultado.DiasEmAtraso);
        Assert.Equal(600.00m, resultado.Juros);
    }

    [Fact]
    public void Nao_aceita_valor_negativo()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CalculadoraJuros.Calcular(-1m, Hoje, Hoje));
    }
}
