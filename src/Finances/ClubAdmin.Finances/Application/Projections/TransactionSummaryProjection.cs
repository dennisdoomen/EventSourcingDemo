using ClubAdmin.Finances.Domain.Events;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ClubAdmin.Finances.Application.Projections;

/// <summary>
/// Projects transaction events to the TransactionSummary read-model table via Dapper.
/// </summary>
public class TransactionSummaryProjection
{
    private readonly string _connectionString;

    public TransactionSummaryProjection(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task On(TransactionImported evt)
    {
        await using var connection = new SqlConnection(_connectionString);

        await connection.ExecuteAsync("""
            INSERT INTO TransactionSummary
                (TransactionId, ExternalId, TransactionDate, Amount, CurrencyCode, Description,
                 CounterPartyIban, CounterPartyName, IsCategorized)
            VALUES
                (@TransactionId, @ExternalId, @TransactionDate, @Amount, @CurrencyCode, @Description,
                 @CounterPartyIban, @CounterPartyName, 0)
            """,
            new
            {
                evt.TransactionId,
                evt.ExternalId,
                evt.TransactionDate,
                evt.Amount,
                evt.CurrencyCode,
                evt.Description,
                evt.CounterPartyIban,
                evt.CounterPartyName
            });
    }

    public async Task On(TransactionCategorized evt)
    {
        await using var connection = new SqlConnection(_connectionString);

        await connection.ExecuteAsync("""
            UPDATE TransactionSummary
            SET BookingCode = @BookingCode, IsCategorized = 1
            WHERE TransactionId = @TransactionId
            """,
            new { evt.TransactionId, evt.BookingCode });
    }
}
