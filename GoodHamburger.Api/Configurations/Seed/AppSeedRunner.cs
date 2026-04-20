using GoodHamburger.Api.Configurations.Seed.Abstraction;
using GoodHamburger.Api.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;

namespace GoodHamburger.Api.Configurations.Seed;

public static class AppSeedRunner
{


    public static async Task TryRunAsync(
        IServiceProvider serviceProvider, ILogger logger, CancellationToken ct = default)
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
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Erro ao aplicar migracoes ou seeds da aplicacao.");
        }
    }
}
