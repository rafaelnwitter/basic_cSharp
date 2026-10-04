using System.Globalization;
using DesafioComercial.Comissao;
using DesafioComercial.Estoque;
using DesafioComercial.Juros;
using DesafioComercial.Leitura;

var cultura = new CultureInfo("pt-BR");
CultureInfo.CurrentCulture = cultura;
CultureInfo.CurrentUICulture = cultura;

var dados = Path.Combine(AppContext.BaseDirectory, "data");
var vendas = LeitorJson.LerVendas(LeitorJson.LerArquivo(Path.Combine(dados, "vendas.json")));
var estoque = new ServicoEstoque(LeitorJson.LerEstoque(LeitorJson.LerArquivo(Path.Combine(dados, "estoque.json"))));

if (args.Contains("--demo"))
{
    ExecutarDemo(vendas, estoque, cultura);
    return;
}

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1 - Comissões do time comercial");
    Console.WriteLine("2 - Movimentação de estoque");
    Console.WriteLine("3 - Juros por atraso");
    Console.WriteLine("0 - Sair");
    Console.Write("Opção: ");

    switch (Console.ReadLine()?.Trim())
    {
        case "1":
            ImprimirComissoes(CalculadoraComissao.Agrupar(vendas), cultura);
            break;
        case "2":
            MovimentarEstoque(estoque, cultura);
            break;
        case "3":
            CalcularJuros(cultura);
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Opção inválida.");
            break;
    }
}

static void ExecutarDemo(IReadOnlyList<Venda> vendas, ServicoEstoque estoque, CultureInfo cultura)
{
    ImprimirComissoes(CalculadoraComissao.Agrupar(vendas), cultura);

    var entrada = estoque.Movimentar(101, TipoMovimentacao.Entrada, 10, "Compra do fornecedor");
    var saida = estoque.Movimentar(101, TipoMovimentacao.Saida, 25, "Venda no balcão");
    ImprimirMovimento(entrada);
    ImprimirMovimento(saida);

    var hoje = DateOnly.FromDateTime(DateTime.Today);
    var exemplo = CalculadoraJuros.Calcular(1500m, hoje.AddDays(-4), hoje);
    ImprimirJuros(exemplo, cultura);
}

static void ImprimirComissoes(IReadOnlyList<ResumoComissaoVendedor> resumos, CultureInfo cultura)
{
    Console.WriteLine();
    Console.WriteLine("Vendedor            Vendas  Total vendido     Comissão");
    foreach (var resumo in resumos)
    {
        Console.WriteLine(
            $"{resumo.Vendedor,-18} {resumo.QuantidadeVendas,6}  {Moeda(resumo.TotalVendido, cultura),14}  {Moeda(resumo.TotalComissao, cultura),12}");
    }

    Console.WriteLine(
        $"{"TOTAL",-18} {resumos.Sum(item => item.QuantidadeVendas),6}  {Moeda(resumos.Sum(item => item.TotalVendido), cultura),14}  {Moeda(resumos.Sum(item => item.TotalComissao), cultura),12}");
}

static void MovimentarEstoque(ServicoEstoque estoque, CultureInfo cultura)
{
    ListarProdutos(estoque);
    Console.Write("Código do produto: ");
    if (!int.TryParse(Console.ReadLine(), out var codigo))
    {
        Console.WriteLine("Código inválido.");
        return;
    }

    Console.Write("Tipo (E entrada / S saída): ");
    var tipoTexto = Console.ReadLine()?.Trim().ToUpperInvariant();
    var tipo = tipoTexto switch
    {
        "E" => TipoMovimentacao.Entrada,
        "S" => TipoMovimentacao.Saida,
        _ => (TipoMovimentacao?)null
    };

    if (tipo is null)
    {
        Console.WriteLine("Tipo inválido.");
        return;
    }

    Console.Write("Quantidade: ");
    if (!int.TryParse(Console.ReadLine(), NumberStyles.Integer, cultura, out var quantidade))
    {
        Console.WriteLine("Quantidade inválida.");
        return;
    }

    Console.Write("Descrição da movimentação: ");
    var descricao = Console.ReadLine() ?? "";

    try
    {
        ImprimirMovimento(estoque.Movimentar(codigo, tipo.Value, quantidade, descricao));
    }
    catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
    {
        Console.WriteLine(ex.Message);
    }
}

static void ListarProdutos(ServicoEstoque estoque)
{
    Console.WriteLine();
    Console.WriteLine("Código  Estoque  Produto");
    foreach (var produto in estoque.Produtos)
        Console.WriteLine($"{produto.Codigo,6}  {produto.Quantidade,7}  {produto.Descricao}");
}

static void ImprimirMovimento(ResultadoMovimentacao resultado)
{
    var movimento = resultado.Movimentacao;
    Console.WriteLine(
        $"Movimento {movimento.Id} | {movimento.Descricao} | {movimento.Tipo} | {resultado.DescricaoProduto} | qtde {movimento.Quantidade} | estoque final {resultado.QuantidadeFinal}");
}

static void CalcularJuros(CultureInfo cultura)
{
    var valor = LerDecimal("Valor original: ", cultura);
    var vencimento = LerData("Vencimento (dd/MM/aaaa): ", cultura);
    var hoje = DateOnly.FromDateTime(DateTime.Today);

    try
    {
        ImprimirJuros(CalculadoraJuros.Calcular(valor, vencimento, hoje), cultura);
    }
    catch (ArgumentOutOfRangeException ex)
    {
        Console.WriteLine(ex.Message);
    }
}

static void ImprimirJuros(ResultadoJuros resultado, CultureInfo cultura)
{
    Console.WriteLine($"Referência: {resultado.DataReferencia:dd/MM/yyyy}");
    Console.WriteLine($"Vencimento: {resultado.Vencimento:dd/MM/yyyy}");
    Console.WriteLine($"Dias em atraso: {resultado.DiasEmAtraso}");
    Console.WriteLine($"Juros (2,5% ao dia, simples): {Moeda(resultado.Juros, cultura)}");
    Console.WriteLine($"Valor atualizado: {Moeda(resultado.ValorAtualizado, cultura)}");
}

static decimal LerDecimal(string prompt, CultureInfo cultura)
{
    while (true)
    {
        Console.Write(prompt);
        var texto = Console.ReadLine();
        if (decimal.TryParse(texto, NumberStyles.Number, cultura, out var valor) ||
            decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out valor))
            return valor;

        Console.WriteLine("Informe um valor numérico.");
    }
}

static DateOnly LerData(string prompt, CultureInfo cultura)
{
    while (true)
    {
        Console.Write(prompt);
        if (DateOnly.TryParse(Console.ReadLine(), cultura, DateTimeStyles.None, out var data))
            return data;

        Console.WriteLine("Data inválida. Use dd/MM/aaaa.");
    }
}

static string Moeda(decimal valor, CultureInfo cultura) => valor.ToString("C2", cultura);
