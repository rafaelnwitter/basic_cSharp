# Desafio Comercial

Este projeto resolve três desafios simples de C#:

1. Calcular a comissão de cada vendedor a partir de um arquivo JSON de vendas.
2. Registrar entradas e saídas de estoque e mostrar a quantidade final de cada produto.
3. Calcular juros de uma conta em atraso, considerando 2,5% ao dia.

A ideia é manter a lógica de negócio separada do programa de console. Assim, dá para testar as regras sem precisar digitar nada no terminal.

## Requisitos

- .NET SDK 10.0

Confirme a versão instalada com:

```bash
dotnet --version
```

## Como rodar

Depois de clonar ou extrair o projeto, entre na pasta:

```bash
cd DesafioComercial
```

Para ver os três desafios funcionando de uma vez, use:

```bash
dotnet run --project src/DesafioComercial.Console -- --demo
```

Esse comando calcula as comissões do arquivo `data/vendas.json`, faz uma entrada e uma saída de estoque e mostra um exemplo de cálculo de juros.

Para usar o programa no modo interativo, rode:

```bash
dotnet run --project src/DesafioComercial.Console
```

Você verá este menu:

```text
1 - Comissões do time comercial
2 - Movimentação de estoque
3 - Juros por atraso
0 - Sair
```

### Comissões

Escolha a opção `1`.

O programa lê `data/vendas.json` e mostra, para cada vendedor, quantas vendas foram feitas, o total vendido e a comissão total.

A regra é calculada venda por venda:

- Abaixo de R$ 100,00: não gera comissão.
- De R$ 100,00 até menos de R$ 500,00: comissão de 1%.
- A partir de R$ 500,00: comissão de 5%.

Por exemplo, uma venda de R$ 1.200,50 gera R$ 60,03 de comissão. Já uma venda de R$ 90,75 não gera comissão.

### Estoque

Escolha a opção `2`.

O programa mostra os produtos disponíveis e pede:

- Código do produto.
- Tipo de movimentação: `E` para entrada ou `S` para saída.
- Quantidade.
- Descrição da movimentação.

Ao final, ele mostra o estoque final daquele produto.

Exemplo:

```text
Código do produto: 101
Tipo (E entrada / S saída): E
Quantidade: 10
Descrição da movimentação: Compra do fornecedor
```

Se tentar retirar mais unidades do que existem, o programa avisa e não deixa o estoque ficar negativo.

### Juros

Escolha a opção `3`.

Informe o valor original e a data de vencimento no formato `dd/MM/aaaa`.

O programa calcula os dias de atraso considerando a data de hoje e aplica juros simples de 2,5% ao dia.

Exemplo:

```text
Valor original: 1000
Vencimento (dd/MM/aaaa): 01/10/2026
```

Se hoje for 03/10/2026, existem 2 dias de atraso. Os juros serão R$ 50,00 e o valor atualizado será R$ 1.050,00.

Se a conta ainda não venceu, ou vence hoje, o juro é zero.

## Como rodar os testes

Para conferir se todas as regras estão funcionando:

```bash
dotnet test DesafioComercial.sln
```

Você deve ver algo como:

```text
Passed! - Failed: 0, Passed: 26, Skipped: 0, Total: 26
```

Os testes cobrem as faixas de comissão, o arquivo de vendas do desafio, entradas e saídas de estoque, estoque insuficiente e o cálculo de juros.

## Organização do projeto

```text
DesafioComercial/
├─ data/
│  ├─ vendas.json
│  └─ estoque.json
├─ src/
│  ├─ DesafioComercial/
│  │  ├─ Comissao/
│  │  ├─ Estoque/
│  │  ├─ Juros/
│  │  └─ Leitura/
│  └─ DesafioComercial.Console/
└─ tests/
   └─ DesafioComercial.Tests/
```

- `data/`: arquivos JSON usados como base do desafio.
- `src/DesafioComercial/`: regras de negócio, sem depender do console.
- `src/DesafioComercial.Console/`: menu, leitura do teclado e exibição dos resultados.
- `tests/DesafioComercial.Tests/`: testes automatizados das regras.

## Premissas adotadas

- A comissão é calculada por venda, e não sobre o total do vendedor.
- Cada comissão é arredondada para duas casas decimais antes de ser somada.
- A movimentação de estoque exige descrição e quantidade maior que zero.
- Uma saída não pode deixar o estoque negativo.
- Os juros são simples: valor original vezes 2,5% vezes dias corridos de atraso.
- O atraso começa no dia seguinte ao vencimento.