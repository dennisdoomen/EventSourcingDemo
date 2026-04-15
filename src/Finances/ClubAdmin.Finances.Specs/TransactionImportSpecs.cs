using ClubAdmin.Finances.Domain;
using ClubAdmin.Finances.Domain.Events;
using FluentAssertions;
using Xunit;

namespace ClubAdmin.Finances.Specs;

public class TransactionImportSpecs
{
    [Fact]
    public void Importing_a_transaction_raises_TransactionImported()
    {
        var (transaction, evt) = Transaction.Import(
            TransactionId.New(),
            externalId: "ING-20240101-001",
            transactionDate: new DateOnly(2024, 1, 1),
            amount: -42.50m,
            currencyCode: "EUR",
            description: "Field rental January",
            counterPartyIban: "NL91ABNA0417164300",
            counterPartyName: "Sports Hall BV",
            rawData: "{}");

        evt.Should().NotBeNull();
        evt.ExternalId.Should().Be("ING-20240101-001");
        evt.Amount.Should().Be(-42.50m);
        transaction.IsCategorized.Should().BeFalse();
    }

    [Fact]
    public void Categorizing_a_transaction_marks_it_as_categorized()
    {
        var (transaction, _) = Transaction.Import(
            TransactionId.New(),
            "ING-20240101-002",
            new DateOnly(2024, 1, 2),
            -100m,
            "EUR",
            "Equipment purchase",
            "NL91ABNA0417164300",
            "Sport Store",
            "{}");

        transaction.Categorize(BookingCode.Equipment, notes: null, categorizedByAi: false);

        transaction.IsCategorized.Should().BeTrue();
        transaction.BookingCode.Should().Be(BookingCode.Equipment);
    }

    [Fact]
    public void Categorizing_an_already_categorized_transaction_throws()
    {
        var (transaction, _) = Transaction.Import(
            TransactionId.New(),
            "ING-20240101-003",
            new DateOnly(2024, 1, 3),
            -50m,
            "EUR",
            "Travel reimbursement",
            "NL91ABNA0417164300",
            "J. Smith",
            "{}");

        transaction.Categorize(BookingCode.Travel, null, false);

        var act = () => transaction.Categorize(BookingCode.Miscellaneous, null, false);
        act.Should().Throw<InvalidOperationException>().WithMessage("*already categorized*");
    }
}
