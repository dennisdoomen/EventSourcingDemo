using ClubAdmin.Members.Application;
using ClubAdmin.Members.Application.Projections;
using Eventuous.SqlServer.Projections;
using Microsoft.Extensions.DependencyInjection;

namespace ClubAdmin.Members.Infrastructure;

/// <summary>
/// Wires up Eventuous with Azure SQL for the Members module.
/// </summary>
public static class EventuousRegistration
{
    public static IServiceCollection AddMembersModule(this IServiceCollection services, string connectionString)
    {
        services.AddEventuousSqlServer(connectionString, "dbo", initializeDatabase: true);

        services.AddSingleton<MemberCommandService>();

        services.AddSingleton<MemberListProjection>(
            _ => new MemberListProjection(new SqlServerConnectionOptions(connectionString, "dbo")));

        return services;
    }
}
