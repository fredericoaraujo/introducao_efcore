using System.Linq;
using CursoEFCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CursoEFCore.Data;

public class ApplicationContext : DbContext
{
    private static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(p => p.AddConsole());
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = "Data Source=localhost;Initial Catalog=CursoEFCore;User Id=sa; Password=Admin#1234; TrustServerCertificate=true";
        optionsBuilder
            .UseLoggerFactory(loggerFactory) // Define qual o logger que eu desejo utilizar
            .EnableSensitiveDataLogging() // Habilita dados do EF para o logger
            .UseSqlServer(connectionString,
                p => p.EnableRetryOnFailure( // habilitando a reconexão com banco de dados
                    maxRetryCount: 2, // quantidade de tentativas de reconexão
                    maxRetryDelay: TimeSpan.FromSeconds(5), // tem para tentativa de reconexão
                    errorNumbersToAdd: null // nenhum código padrão para interpretação
                )
                .MigrationsHistoryTable("curso_ef_core") // definindo o nome da tabela de histórico de migrações
            );

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
        MapearPropriedadesEsquecidas(modelBuilder);
    }

    private void MapearPropriedadesEsquecidas(ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entity.GetProperties().Where(p => p.ClrType == typeof(string));
            foreach (var property in properties)
            {
                if (string.IsNullOrEmpty(property.GetColumnType())
                    && !property.GetMaxLength().HasValue)
                {
                    // property.SetMaxLength(100);
                    property.SetColumnType("VARCHAR(100)");
                }
            }
        }
    }


}
