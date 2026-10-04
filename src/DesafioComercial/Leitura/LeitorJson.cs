using System.Text.Json;
using DesafioComercial.Comissao;
using DesafioComercial.Estoque;

namespace DesafioComercial.Leitura;

public static class LeitorJson
{
    private static readonly JsonSerializerOptions Opcoes = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static IReadOnlyList<Venda> LerVendas(string json)
    {
        var documento = Desserializar<VendasArquivo>(json, "vendas");
        return documento.Vendas
            .Select(item => new Venda(item.Vendedor, item.Valor))
            .ToArray();
    }

    public static IReadOnlyList<ProdutoEstoque> LerEstoque(string json)
    {
        var documento = Desserializar<EstoqueArquivo>(json, "estoque");
        return documento.Estoque
            .Select(item => new ProdutoEstoque(item.CodigoProduto, item.DescricaoProduto, item.Estoque))
            .ToArray();
    }

    public static string LerArquivo(string caminho)
    {
        if (!File.Exists(caminho))
            throw new FileNotFoundException("Arquivo JSON não encontrado.", caminho);

        return File.ReadAllText(caminho);
    }

    private static T Desserializar<T>(string json, string nome) where T : class
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException($"JSON de {nome} vazio.", nameof(json));

        return JsonSerializer.Deserialize<T>(json, Opcoes)
            ?? throw new InvalidOperationException($"JSON de {nome} inválido.");
    }

    private sealed class VendasArquivo
    {
        public List<VendaJson> Vendas { get; init; } = [];
    }

    private sealed class VendaJson
    {
        public string Vendedor { get; init; } = "";
        public decimal Valor { get; init; }
    }

    private sealed class EstoqueArquivo
    {
        public List<ProdutoJson> Estoque { get; init; } = [];
    }

    private sealed class ProdutoJson
    {
        public int CodigoProduto { get; init; }
        public string DescricaoProduto { get; init; } = "";
        public int Estoque { get; init; }
    }
}
