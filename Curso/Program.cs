using System.Runtime.InteropServices;
using CursoEFCore.Data;
using CursoEFCore.Domain;
using CursoEFCore.ValueObjects;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");


        //InserirProdutos();
        //InserirDadosEmMassa();
        //ConsultarDados();

        //CadastrarPedido();

        //ConsultaPedidoCarregamentoAdiantado();

        //AtualizarDadosDesconectados();

        RemoverRegistroDesconectado();
    }

    private static void RemoverRegistro()
    {
        using var db = new ApplicationContext();
        var cliente = db.Clientes.Find(2);
        // db.Clientes.Remove(cliente);
        // db.Remove(cliente);
        db.Entry(cliente).State = EntityState.Deleted;
        db.SaveChanges();
    }

    private static void RemoverRegistroDesconectado()
    {
        using var db = new ApplicationContext();
        var cliente = new Cliente { Id = 3 };
        // db.Clientes.Remove(cliente);
        // db.Remove(cliente);
        db.Entry(cliente).State = EntityState.Deleted;
        db.SaveChanges();
    }

    private static void AtualizarDados()
    {
        using var db = new ApplicationContext();
        var cliente = db.Clientes.Find(1);
        cliente.Nome = "Nome alterado";
        // db.Clientes.Update(cliente); // atualiza todas as propriedades da entidade
        db.SaveChanges();
    }

    private static void AtualizarDadosDesconectados()
    {
        using var db = new ApplicationContext();
        var cliente = new Cliente
        {
            Id = 1
        };
        var clienteDesconectado = new
        {
            Nome = "Cliente Desconectado",
            Telefone = "09876543211"
        };
        db.Attach(cliente);
        db.Entry(cliente).CurrentValues.SetValues(clienteDesconectado);
        db.SaveChanges();
    }


    private static void ConsultaPedidoCarregamentoAdiantado()
    {
        using var db = new ApplicationContext();
        var pedidos = db.Pedidos
                            .Include(p => p.Itens)
                            .ThenInclude(p => p.Produto)
                            .Include(p => p.Cliente)
                            .ToList();
        Console.WriteLine(pedidos.Count);
    }

    private static void CadastrarPedido()
    {
        using var applicationContext = new ApplicationContext();
        var cliente = applicationContext.Clientes.FirstOrDefault();
        var produto = applicationContext.Produtos.FirstOrDefault();

        var pedido = new Pedido
        {
            ClienteId = cliente.Id,
            IniciadoEm = DateTime.Now,
            FinalizadoEm = DateTime.Now,
            Observacao = "Pedido teste",
            StatusPedido = StatusPedido.Analise,
            TipoFrete = TipoFrete.SemFrete,
            Itens = new List<PedidoItem>
            {
                new PedidoItem
                {
                    ProdutoId = produto.Id,
                    Desconto = 0,
                    Quantidade = 1,
                    Valor = 10
                }
            }
        };

        applicationContext.Pedidos.Add(pedido);
        applicationContext.SaveChanges();

    }

    private static void InserirProdutos()
    {
        Produto produto = new Produto
        {
            Descricao = "Produto Teste",
            CodigoBarras = "1234567890",
            Valor = 10m,
            TipoProduto = TipoProduto.MercadoriaParaRevenda,
            Ativo = true
        };

        using ApplicationContext db = new ApplicationContext();
        // 1a opção de adicionar um item
        db.Produtos.Add(produto);
        // 2a opção de adicionar um produto
        // db.Set<Produto>().Add(produto);
        // 3a opção - vai procurar a sua entidade dentro do Application pelo tipo
        // db.Entry(produto).State = EntityState.Added;
        // 4a opção - sobrecarga
        // db.Add(produto);
        // As opções 1 e 2 são as mais indicadas

        // db.SaveChanges() -> aplica todas as alterações que estão sendo gerenciadas naquele escopo
        var registros = db.SaveChanges();
        Console.WriteLine($"Total de registros afetados: {registros}");
    }

    private static void InserirDadosEmMassa()
    {
        Produto produto = new Produto
        {
            Descricao = "Produto Teste 2",
            CodigoBarras = "12345678901234",
            Valor = 10m,
            TipoProduto = TipoProduto.MercadoriaParaRevenda,
            Ativo = true
        };

        Cliente cliente = new Cliente
        {
            Nome = "Cliente Teste",
            CEP = "12345678",
            Cidade = "Aparecida do Norte",
            Estado = "SP",
            Telefone = "12345678901"
        };

        using ApplicationContext applicationContext = new ApplicationContext();
        applicationContext.AddRange(produto, cliente);

        var qtdeRegistros = applicationContext.SaveChanges();
        Console.WriteLine($"Total de registros: {qtdeRegistros}");
    }

    private static void ConsultarDados()
    {
        using ApplicationContext applicationContext = new ApplicationContext();
        // opção utilizando o linq
        // var consultaPorSintaxe = (from c in applicationContext.Clientes where c.Id > 0 select c).ToList();
        // opção mais aceita
        var consultaPorMetodos = applicationContext.Clientes.AsNoTracking().Where(p => p.Id > 0).ToList();

        foreach (Cliente cliente in consultaPorMetodos)
        {
            Console.WriteLine($"Cliente: {cliente.Id}");
            // O método .Find faz uma por Id
            // Ele primeiro procura o objeto em memória
            // Se não encontrar vai buscar no banco de dados
            applicationContext.Clientes.Find(cliente.Id);
        }

    }
}