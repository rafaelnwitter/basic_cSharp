namespace DesafioComercial.Estoque;

public sealed class ServicoEstoque
{
    private readonly Dictionary<int, ProdutoEstoque> _produtos;
    private readonly List<MovimentacaoEstoque> _movimentacoes = [];
    private long _proximoId = 1;

    public ServicoEstoque(IEnumerable<ProdutoEstoque> produtos)
    {
        ArgumentNullException.ThrowIfNull(produtos);

        _produtos = [];
        foreach (var produto in produtos)
        {
            if (string.IsNullOrWhiteSpace(produto.Descricao))
                throw new ArgumentException("A descrição do produto é obrigatória.");

            if (produto.Quantidade < 0)
                throw new ArgumentOutOfRangeException(nameof(produtos), "O estoque inicial não pode ser negativo.");

            if (!_produtos.TryAdd(produto.Codigo, produto))
                throw new ArgumentException($"Produto duplicado: {produto.Codigo}.");
        }
    }

    public IReadOnlyList<ProdutoEstoque> Produtos =>
        _produtos.Values.OrderBy(produto => produto.Codigo).ToArray();

    public IReadOnlyList<MovimentacaoEstoque> Historico => _movimentacoes;

    public ResultadoMovimentacao Movimentar(
        int codigoProduto,
        TipoMovimentacao tipo,
        int quantidade,
        string descricao,
        DateTime? realizadaEm = null)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("A descrição da movimentação é obrigatória.", nameof(descricao));

        if (quantidade <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantidade), "A quantidade deve ser maior que zero.");

        if (!_produtos.TryGetValue(codigoProduto, out var produto))
            throw new InvalidOperationException($"Produto {codigoProduto} não encontrado.");

        if (!Enum.IsDefined(tipo))
            throw new ArgumentOutOfRangeException(nameof(tipo), "Tipo de movimentação inválido.");

        var quantidadeFinal = tipo switch
        {
            TipoMovimentacao.Entrada => produto.Quantidade + quantidade,
            TipoMovimentacao.Saida => Baixar(produto, quantidade),
            _ => throw new ArgumentOutOfRangeException(nameof(tipo))
        };

        var atualizado = produto with { Quantidade = quantidadeFinal };
        _produtos[codigoProduto] = atualizado;

        var movimentacao = new MovimentacaoEstoque(
            _proximoId++,
            descricao.Trim(),
            tipo,
            codigoProduto,
            quantidade,
            realizadaEm ?? DateTime.Now);

        _movimentacoes.Add(movimentacao);
        return new ResultadoMovimentacao(movimentacao, atualizado.Descricao, quantidadeFinal);
    }

    private static int Baixar(ProdutoEstoque produto, int quantidade)
    {
        if (produto.Quantidade < quantidade)
        {
            throw new InvalidOperationException(
                $"Estoque insuficiente de {produto.Descricao}. Disponível: {produto.Quantidade}. Solicitado: {quantidade}.");
        }

        return produto.Quantidade - quantidade;
    }
}
