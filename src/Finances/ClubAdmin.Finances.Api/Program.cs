using ClubAdmin.Finances.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolverine;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        var connectionString = context.Configuration["SqlConnectionString"]
            ?? throw new InvalidOperationException("SqlConnectionString is not configured.");

        var eventStoreConnectionString = context.Configuration["EventStoreConnectionString"]
            ?? throw new InvalidOperationException("EventStoreConnectionString is not configured.");

        services.AddFinancesModule(connectionString, eventStoreConnectionString);
    })
    .UseWolverine(opts =>
    {
        opts.AddFinancesHandlers();
    })
    .Build();

await host.RunAsync();
