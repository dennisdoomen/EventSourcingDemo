using ClubAdmin.Members.Application;
using ClubAdmin.Members.Application.Projections;
using Eventuous;
using Eventuous.SqlServer;
using Microsoft.Extensions.DependencyInjection;

namespace ClubAdmin.Members.Infrastructure;

/// <summary>
/// Wires up Eventuous with Azure SQL for the Members module.
/// </summary>
public static class EventuousRegistration
{
    public static IServiceCollection AddMembersModule(this IServiceCollection services, string connectionString)
    {
        services.AddEventStoreClient<SqlServerEventStore>(sp =>
            new SqlServerEventStore(connectionString));

        services.AddCommandService<MemberCommandService>();

        services.AddSingleton<MemberListProjection>(_ => new MemberListProjection(connectionString));

        return services;
    }
}
