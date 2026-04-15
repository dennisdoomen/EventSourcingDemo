namespace ClubAdmin.Finances.Application.Commands;

public record ImportTransaction(
    string ExternalId,
    DateOnly TransactionDate,
    decimal Amount,
    string CurrencyCode,
    string Description,
    string CounterPartyIban,
    string CounterPartyName,
    string RawData);

public record CategorizeTransaction(
    string TransactionId,
    string BookingCode,
    string? Notes,
    bool CategorizedByAi = false);

public record DefineBudget(
    int Year,
    string BookingCode,
    decimal BudgetAmount,
    string CurrencyCode = "EUR");

public record AllocateBudget(
    int Year,
    string BookingCode,
    decimal Amount,
    string CurrencyCode,
    string Reason);
