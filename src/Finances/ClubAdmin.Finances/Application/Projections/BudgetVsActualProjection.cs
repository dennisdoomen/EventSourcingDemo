using ClubAdmin.Finances.Domain.Events;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ClubAdmin.Finances.Application.Projections;

/// <summary>
/// Projects budget events to the BudgetVsActual read-model table via Dapper.
/// </summary>
public class BudgetVsActualProjection
{
    private readonly string _connectionString;

    public BudgetVsActualProjection(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task On(BudgetDefined evt)
    {
        await using var connection = new SqlConnection(_connectionString);

        await connection.ExecuteAsync("""
            MERGE BudgetVsActual AS target
            USING (SELECT @Year, @BookingCode) AS source (Year, BookingCode)
            ON target.Year = source.Year AND target.BookingCode = source.BookingCode
            WHEN MATCHED THEN
                UPDATE SET PlannedAmount = @PlannedAmount, CurrencyCode = @CurrencyCode
            WHEN NOT MATCHED THEN
                INSERT (Year, BookingCode, PlannedAmount, ActualAmount, CurrencyCode)
                VALUES (@Year, @BookingCode, @PlannedAmount, 0, @CurrencyCode);
            """,
            new { evt.Year, evt.BookingCode, PlannedAmount = evt.BudgetAmount, evt.CurrencyCode });
    }

    public async Task On(TransactionCategorized evt)
    {
        // Actual spend is updated when a transaction is categorized.
        // The full transaction amount needs to come from the event store or read-model join.
        // This is a placeholder — a real implementation would look up the transaction amount.
    }
}
