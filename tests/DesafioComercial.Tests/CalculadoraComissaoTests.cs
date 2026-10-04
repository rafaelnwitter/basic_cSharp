using Xunit;
using DesafioComercial.Comissao;

namespace DesafioComercial.Tests;

public class CalculadoraComissaoTests
{
    [Theory]
    [InlineData("0", "0.00")]
    [InlineData("99.99", "0.00")]
    [InlineData("100.00", "1.00")]
    [InlineData("250.30", "2.50")]
    [InlineData("499.99", "5.00")]
    [InlineData("500.00", "25.00")]
    [InlineData("1200.50", "60.03")]
    public void Aplica_faixa_por_venda(string valor, string esperado)
    {
        var comissao = CalculadoraComissao.Calcular(decimal.Parse(valor, System.Globalization.CultureInfo.InvariantCulture));

        Assert.Equal(decimal.Parse(esperado, System.Globalization.CultureInfo.InvariantCulture), comissao);
    }

    [Fact]
    public void Nao_aceita_venda_negativa()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CalculadoraComissao.Calcular(-0.01m));
    }

    [Fact]
    public void Soma_comissao_de_cada_venda_e_nao_do_total()
    {
        var resumo = Assert.Single(CalculadoraComissao.Agrupar(
        [
            new Venda("Ana", 300m),
            new Venda("Ana", 300m)
        ]));

        Assert.Equal(6.00m, resumo.TotalComissao);
        Assert.Equal(600m, resumo.TotalVendido);
    }

    [Fact]
    public void Mantem_a_ordem_de_aparecimento_dos_vendedores()
    {
        var nomes = CalculadoraComissao.Agrupar(
        [
            new Venda("Maria", 100m),
            new Venda("João", 500m),
            new Venda("Maria", 50m)
        ]).Select(item => item.Vendedor);

        Assert.Equal(["Maria", "João"], nomes);
    }

    [Fact]
    public void Vendedor_em_branco_e_invalido()
    {
        Assert.Throws<ArgumentException>(() => CalculadoraComissao.Agrupar([new Venda("  ", 100m)]));
    }
}
