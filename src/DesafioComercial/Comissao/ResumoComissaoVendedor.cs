namespace DesafioComercial.Comissao;

public sealed record ResumoComissaoVendedor(
    string Vendedor,
    int QuantidadeVendas,
    decimal TotalVendido,
    decimal TotalComissao,
    IReadOnlyList<ItemComissao> Itens);
