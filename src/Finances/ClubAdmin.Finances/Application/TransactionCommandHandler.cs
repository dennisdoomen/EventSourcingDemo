using ClubAdmin.Finances.Application.Commands;
using ClubAdmin.Finances.Domain;
using ClubAdmin.Finances.Domain.Events;
using Wolverine;

namespace ClubAdmin.Finances.Application;

/// <summary>
/// Wolverine message handler for financial transaction commands.
/// </summary>
public static class TransactionCommandHandler
{
    public static (TransactionImported, OutgoingMessages) Handle(ImportTransaction cmd)
    {
        var (_, evt) = Transaction.Import(
            TransactionId.New(),
            cmd.ExternalId,
            cmd.TransactionDate,
            cmd.Amount,
            cmd.CurrencyCode,
            cmd.Description,
            cmd.CounterPartyIban,
            cmd.CounterPartyName,
            cmd.RawData);

        return (evt, []);
    }

    public static TransactionCategorized Handle(CategorizeTransaction cmd, Transaction transaction)
    {
        var bookingCode = Enum.Parse<BookingCode>(cmd.BookingCode);
        return transaction.Categorize(bookingCode, cmd.Notes, cmd.CategorizedByAi);
    }
}
