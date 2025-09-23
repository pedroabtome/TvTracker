using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TvTracker.Api;
using TvTracker.Infrastructure.Persistence;
using System.Linq;

namespace TvTracker.Tests.Integration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private SqliteConnection? _connection;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing"); // força o env "Testing" na API durante os testes

            builder.ConfigureServices(services =>
            {
                // 1) Remover qualquer registo do DbContext que porventura tenha escapado
                var toRemove = services
                    .Where(d => d.ServiceType == typeof(DbContextOptions<TrackerDbContext>)
                             || d.ServiceType == typeof(TrackerDbContext))
                    .ToList();
                foreach (var d in toRemove) services.Remove(d);

                // 2) Abrir uma ligação Sqlite in-memory PARTILHADA e mantê-la viva
                _connection = new SqliteConnection("Data Source=:memory:;Cache=Shared");
                _connection.Open();

                // 3) Registrar o DbContext dos TESTES só com Sqlite
                services.AddDbContext<TrackerDbContext>(options =>
                {
                    options.UseSqlite(_connection);
                });

                // 4) Criar o schema
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<TrackerDbContext>();
                db.Database.EnsureCreated();
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _connection?.Dispose();
                _connection = null;
            }
        }
    }
}
