using ClubAdmin.Finances.Application.Projections;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;

namespace ClubAdmin.Finances.Infrastructure;

/// <summary>
/// Wires up Wolverine, EventStoreDB, and Liquid Projections for the Finances module.
/// </summary>
public static class WolverineRegistration
{
    public static IServiceCollection AddFinancesModule(
        this IServiceCollection services,
        string connectionString,
        string eventStoreConnectionString)
    {
        services.AddSingleton(new EventStoreClientSettings { ConnectivitySettings = { Address = new Uri(eventStoreConnectionString) } });
        services.AddSingleton<EventStoreClient>(sp =>
            new EventStoreClient(sp.GetRequiredService<EventStoreClientSettings>()));

        services.AddSingleton<TransactionSummaryProjection>(_ =>
            new TransactionSummaryProjection(connectionString));

        services.AddSingleton<BudgetVsActualProjection>(_ =>
            new BudgetVsActualProjection(connectionString));

        return services;
    }

    public static WolverineOptions AddFinancesHandlers(this WolverineOptions opts)
    {
        opts.Discovery.IncludeAssembly(typeof(WolverineRegistration).Assembly);
        return opts;
    }
}
