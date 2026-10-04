namespace DesafioComercial.Estoque;

public sealed record MovimentacaoEstoque(
    long Id,
    string Descricao,
    TipoMovimentacao Tipo,
    int CodigoProduto,
    int Quantidade,
    DateTime RealizadaEm);
