using ClubAdmin.Finances.Domain.Events;

namespace ClubAdmin.Finances.Domain;

/// <summary>
/// Represents an imported bank transaction. State is event-sourced via Wolverine.
/// </summary>
public class Transaction
{
    public TransactionId Id { get; private set; }

    public string ExternalId { get; private set; } = string.Empty;

    public DateOnly TransactionDate { get; private set; }

    public decimal Amount { get; private set; }

    public string CurrencyCode { get; private set; } = "EUR";

    public string Description { get; private set; } = string.Empty;

    public BookingCode? BookingCode { get; private set; }

    public bool IsCategorized => BookingCode.HasValue;

    public static (Transaction, TransactionImported) Import(
        TransactionId transactionId,
        string externalId,
        DateOnly transactionDate,
        decimal amount,
        string currencyCode,
        string description,
        string counterPartyIban,
        string counterPartyName,
        string rawData)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(externalId, nameof(externalId));

        var transaction = new Transaction();
        var evt = new TransactionImported(
            transactionId.ToString(),
            externalId,
            transactionDate,
            amount,
            currencyCode,
            description,
            counterPartyIban,
            counterPartyName,
            rawData);

        transaction.Apply(evt);
        return (transaction, evt);
    }

    public TransactionCategorized Categorize(BookingCode bookingCode, string? notes, bool categorizedByAi)
    {
        if (IsCategorized)
        {
            throw new InvalidOperationException("Transaction is already categorized.");
        }

        var evt = new TransactionCategorized(Id.ToString(), bookingCode.ToString(), notes, categorizedByAi);
        Apply(evt);
        return evt;
    }

    private void Apply(TransactionImported evt)
    {
        Id = TransactionId.Parse(evt.TransactionId);
        ExternalId = evt.ExternalId;
        TransactionDate = evt.TransactionDate;
        Amount = evt.Amount;
        CurrencyCode = evt.CurrencyCode;
        Description = evt.Description;
    }

    private void Apply(TransactionCategorized evt)
    {
        BookingCode = Enum.Parse<BookingCode>(evt.BookingCode);
    }
}
