using GoodHamburger.Api.Configurations.Seed.Abstraction;
using GoodHamburger.Api.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using System.Net.Sockets;

namespace GoodHamburger.Api.Configurations.Seed;

public static class AppSeedRunner
{
    private const int MaxAttempts = 10;

    public static async Task TryRunAsync(
        IServiceProvider serviceProvider, ILogger logger, CancellationToken ct = default)
    {
        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var scopedServices = scope.ServiceProvider;

            try
            {
                var dbContext = scopedServices.GetRequiredService<GoodHamburgerDbContext>();
                var databaseCreator = dbContext.Database.GetService<IRelationalDatabaseCreator>();
                if (!await databaseCreator.ExistsAsync(ct))
                {
                    await databaseCreator.CreateAsync(ct);
                }

                var historyRepository = dbContext.Database.GetService<IHistoryRepository>();
                await historyRepository.CreateIfNotExistsAsync(ct);

                await dbContext.Database.MigrateAsync(ct);
                logger.LogInformation("Migracoes aplicadas com sucesso.");

                var seeds = scopedServices.GetServices<IAppSeed>();
                foreach (var seed in seeds)
                {
                    await seed.SeedAsync(scopedServices, ct);
                }

                logger.LogInformation("Seeds aplicadas com sucesso.");
                return;
            }
            catch (Exception exception) when (attempt < MaxAttempts && IsDatabaseStartupFailure(exception))
            {
                var delay = TimeSpan.FromSeconds(Math.Min(attempt * 2, 15));
                logger.LogWarning(
                    exception,
                    "Banco indisponivel ao aplicar migracoes ou seeds. Tentativa {Attempt}/{MaxAttempts}. Nova tentativa em {DelaySeconds}s.",
                    attempt,
                    MaxAttempts,
                    delay.TotalSeconds);

                await Task.Delay(delay, ct);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Erro ao aplicar migracoes ou seeds da aplicacao.");
                throw;
            }
        }
    }

    private static bool IsDatabaseStartupFailure(Exception exception)
    {
        return exception switch
        {
            SocketException => true,
            TimeoutException => true,
            PostgresException { SqlState: PostgresErrorCodes.CannotConnectNow } => true,
            NpgsqlException when exception.InnerException is not null => IsDatabaseStartupFailure(exception.InnerException),
            _ when exception.InnerException is not null => IsDatabaseStartupFailure(exception.InnerException),
            _ => false
        };
    }
}
