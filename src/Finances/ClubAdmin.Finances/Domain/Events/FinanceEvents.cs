namespace ClubAdmin.Finances.Domain.Events;

public record TransactionImported(
    string TransactionId,
    string ExternalId,
    DateOnly TransactionDate,
    decimal Amount,
    string CurrencyCode,
    string Description,
    string CounterPartyIban,
    string CounterPartyName,
    string RawData);

public record TransactionCategorized(
    string TransactionId,
    string BookingCode,
    string? Notes,
    bool CategorizedByAi);

public record BudgetDefined(
    int Year,
    string BookingCode,
    decimal BudgetAmount,
    string CurrencyCode);

public record BudgetAllocated(
    int Year,
    string BookingCode,
    decimal AllocatedAmount,
    string CurrencyCode,
    string Reason);
