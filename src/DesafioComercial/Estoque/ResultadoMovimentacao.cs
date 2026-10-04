namespace DesafioComercial.Estoque;

public sealed record ResultadoMovimentacao(
    MovimentacaoEstoque Movimentacao,
    string DescricaoProduto,
    int QuantidadeFinal);
