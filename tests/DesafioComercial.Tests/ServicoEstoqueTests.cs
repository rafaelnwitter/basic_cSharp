using Xunit;
using DesafioComercial.Estoque;
using DesafioComercial.Leitura;

namespace DesafioComercial.Tests;

public class ServicoEstoqueTests
{
    [Fact]
    public void Entrada_e_saida_devolvem_estoque_final_e_ids_unicos()
    {
        var servico = Criar();
        var quando = new DateTime(2026, 10, 3, 21, 0, 0);

        var entrada = servico.Movimentar(101, TipoMovimentacao.Entrada, 10, "  Compra  ", quando);
        var saida = servico.Movimentar(101, TipoMovimentacao.Saida, 40, "Venda", quando);

        Assert.Equal(1, entrada.Movimentacao.Id);
        Assert.Equal(2, saida.Movimentacao.Id);
        Assert.NotEqual(entrada.Movimentacao.Id, saida.Movimentacao.Id);
        Assert.Equal("Compra", entrada.Movimentacao.Descricao);
        Assert.Equal(TipoMovimentacao.Entrada, entrada.Movimentacao.Tipo);
        Assert.Equal(160, entrada.QuantidadeFinal);
        Assert.Equal(120, saida.QuantidadeFinal);
        Assert.Equal("Caneta Azul", saida.DescricaoProduto);
        Assert.Equal(120, servico.Produtos.Single(item => item.Codigo == 101).Quantidade);
    }

    [Fact]
    public void Saida_pode_zerar_o_estoque()
    {
        var servico = Criar();

        var resultado = servico.Movimentar(105, TipoMovimentacao.Saida, 90, "Inventário zerado");

        Assert.Equal(0, resultado.QuantidadeFinal);
    }

    [Fact]
    public void Nao_permite_estoque_negativo()
    {
        var servico = Criar();

        var erro = Assert.Throws<InvalidOperationException>(() =>
            servico.Movimentar(102, TipoMovimentacao.Saida, 76, "Venda acima do saldo"));

        Assert.Contains("insuficiente", erro.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(75, servico.Produtos.Single(item => item.Codigo == 102).Quantidade);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Quantidade_da_movimentacao_deve_ser_positiva(int quantidade)
    {
        var servico = Criar();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            servico.Movimentar(101, TipoMovimentacao.Entrada, quantidade, "Ajuste"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Descricao_e_obrigatoria(string descricao)
    {
        var servico = Criar();

        Assert.Throws<ArgumentException>(() =>
            servico.Movimentar(101, TipoMovimentacao.Entrada, 1, descricao));
    }

    [Fact]
    public void Produto_inexistente_nao_movimenta()
    {
        var servico = Criar();

        Assert.Throws<InvalidOperationException>(() =>
            servico.Movimentar(999, TipoMovimentacao.Entrada, 1, "Produto novo"));
    }

    [Fact]
    public void Carrega_saldo_inicial_do_json()
    {
        var caminho = Path.Combine(AppContext.BaseDirectory, "data", "estoque.json");
        var servico = new ServicoEstoque(LeitorJson.LerEstoque(LeitorJson.LerArquivo(caminho)));

        Assert.Equal(
            [(101, 150), (102, 75), (103, 200), (104, 320), (105, 90)],
            servico.Produtos.Select(item => (item.Codigo, item.Quantidade)));
    }

    private static ServicoEstoque Criar()
    {
        return new ServicoEstoque(
        [
            new ProdutoEstoque(101, "Caneta Azul", 150),
            new ProdutoEstoque(102, "Caderno Universitário", 75),
            new ProdutoEstoque(105, "Marcador de Texto Amarelo", 90)
        ]);
    }
}
