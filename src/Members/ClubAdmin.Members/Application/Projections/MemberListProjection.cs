using ClubAdmin.Members.Domain.Events;
using Eventuous;
using Eventuous.SqlServer.Projections;
using Eventuous.Subscriptions.Context;
using Microsoft.Data.SqlClient;

namespace ClubAdmin.Members.Application.Projections;

/// <summary>
/// Projects member events to the Members read-model table in Azure SQL.
/// </summary>
public class MemberListProjection : SqlServerProjector
{
    public MemberListProjection(SqlServerConnectionOptions options)
        : base(options, TypeMap.Instance)
    {
        On<MemberRegistered>(Project);
        On<MemberProfileUpdated>(Project);
        On<MembershipTerminated>(Project);
    }

    private static SqlCommand Project(SqlConnection connection, MessageConsumeContext<MemberRegistered> context)
    {
        var evt = context.Message!;

        const string sql = """
            MERGE Members AS target
            USING (SELECT @MemberId, @ClubId, @FirstName, @LastName, @DateOfBirth, @MembershipType, @RegisteredOn)
                AS source (MemberId, ClubId, FirstName, LastName, DateOfBirth, MembershipType, RegisteredOn)
            ON target.MemberId = source.MemberId
            WHEN NOT MATCHED THEN
                INSERT (MemberId, ClubId, FirstName, LastName, DateOfBirth, MembershipType, RegisteredOn, IsActive)
                VALUES (source.MemberId, source.ClubId, source.FirstName, source.LastName,
                        source.DateOfBirth, source.MembershipType, source.RegisteredOn, 1);
            """;

        var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@MemberId", evt.MemberId);
        cmd.Parameters.AddWithValue("@ClubId", evt.ClubId);
        cmd.Parameters.AddWithValue("@FirstName", evt.FirstName);
        cmd.Parameters.AddWithValue("@LastName", evt.LastName);
        cmd.Parameters.AddWithValue("@DateOfBirth", evt.DateOfBirth.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@MembershipType", evt.MembershipType);
        cmd.Parameters.AddWithValue("@RegisteredOn", evt.RegisteredOn.ToDateTime(TimeOnly.MinValue));
        return cmd;
    }

    private static SqlCommand Project(SqlConnection connection, MessageConsumeContext<MemberProfileUpdated> context)
    {
        var evt = context.Message!;

        const string sql = """
            UPDATE Members
            SET FirstName = @FirstName, LastName = @LastName, DateOfBirth = @DateOfBirth
            WHERE MemberId = @MemberId;
            """;

        var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@MemberId", evt.MemberId);
        cmd.Parameters.AddWithValue("@FirstName", evt.FirstName);
        cmd.Parameters.AddWithValue("@LastName", evt.LastName);
        cmd.Parameters.AddWithValue("@DateOfBirth", evt.DateOfBirth.ToDateTime(TimeOnly.MinValue));
        return cmd;
    }

    private static SqlCommand Project(SqlConnection connection, MessageConsumeContext<MembershipTerminated> context)
    {
        var evt = context.Message!;

        const string sql = """
            UPDATE Members
            SET IsActive = 0, TerminatedOn = @EndDate
            WHERE MemberId = @MemberId;
            """;

        var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@MemberId", evt.MemberId);
        cmd.Parameters.AddWithValue("@EndDate", evt.EndDate.ToDateTime(TimeOnly.MinValue));
        return cmd;
    }
}
