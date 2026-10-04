using Xunit;
using DesafioComercial.Comissao;
using DesafioComercial.Leitura;

namespace DesafioComercial.Tests;

public class VendasJsonTests
{
    [Fact]
    public void Calcula_comissao_do_arquivo_do_desafio()
    {
        var caminho = Path.Combine(AppContext.BaseDirectory, "data", "vendas.json");
        var resumos = CalculadoraComissao.Agrupar(LeitorJson.LerVendas(LeitorJson.LerArquivo(caminho)));

        Assert.Equal(
            [
                ("João Silva", 10, 10754.70m, 495.69m),
                ("Maria Souza", 9, 9874.30m, 465.96m),
                ("Carlos Oliveira", 8, 7928.35m, 379.38m),
                ("Ana Lima", 9, 8763.95m, 404.99m)
            ],
            resumos.Select(item => (item.Vendedor, item.QuantidadeVendas, item.TotalVendido, item.TotalComissao)));

        Assert.Equal(1746.02m, resumos.Sum(item => item.TotalComissao));
        Assert.Equal(37321.30m, resumos.Sum(item => item.TotalVendido));
        Assert.Contains(resumos.Single(item => item.Vendedor == "Maria Souza").Itens, item => item.ValorVenda == 90.75m && item.Comissao == 0m);
        Assert.Contains(resumos.Single(item => item.Vendedor == "Carlos Oliveira").Itens, item => item.ValorVenda == 500m && item.Comissao == 25m);
    }
}
