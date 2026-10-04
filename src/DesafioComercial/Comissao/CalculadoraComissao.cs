namespace DesafioComercial.Comissao;

/// <summary>
/// Comissão é calculada por venda, nunca sobre o total do vendedor.
/// Abaixo de 100: 0%. De 100 até menos de 500: 1%. A partir de 500: 5%.
/// Cada comissão é arredondada para 2 casas (meio para cima) antes de somar.
/// </summary>
public static class CalculadoraComissao
{
    public const decimal LimiteSemComissao = 100m;
    public const decimal LimiteComissaoCheia = 500m;
    public const decimal AliquotaIntermediaria = 0.01m;
    public const decimal AliquotaCheia = 0.05m;

    public static decimal Calcular(decimal valor)
    {
        if (valor < 0)
            throw new ArgumentOutOfRangeException(nameof(valor), "O valor da venda não pode ser negativo.");

        if (valor < LimiteSemComissao)
            return 0m;

        var aliquota = valor < LimiteComissaoCheia
            ? AliquotaIntermediaria
            : AliquotaCheia;

        return decimal.Round(valor * aliquota, 2, MidpointRounding.AwayFromZero);
    }

    public static IReadOnlyList<ResumoComissaoVendedor> Agrupar(IEnumerable<Venda> vendas)
    {
        ArgumentNullException.ThrowIfNull(vendas);

        var ordem = new List<string>();
        var porVendedor = new Dictionary<string, List<Venda>>(StringComparer.Ordinal);

        foreach (var venda in vendas)
        {
            if (string.IsNullOrWhiteSpace(venda.Vendedor))
                throw new ArgumentException("O vendedor é obrigatório.", nameof(vendas));

            if (!porVendedor.TryGetValue(venda.Vendedor, out var lista))
            {
                lista = [];
                porVendedor[venda.Vendedor] = lista;
                ordem.Add(venda.Vendedor);
            }

            _ = Calcular(venda.Valor);
            lista.Add(venda);
        }

        return ordem
            .Select(nome => Resumir(nome, porVendedor[nome]))
            .ToArray();
    }

    private static ResumoComissaoVendedor Resumir(string vendedor, IReadOnlyList<Venda> vendas)
    {
        var itens = vendas
            .Select(venda => new ItemComissao(venda.Valor, Calcular(venda.Valor)))
            .ToArray();

        return new ResumoComissaoVendedor(
            vendedor,
            itens.Length,
            itens.Sum(item => item.ValorVenda),
            itens.Sum(item => item.Comissao),
            itens);
    }
}
