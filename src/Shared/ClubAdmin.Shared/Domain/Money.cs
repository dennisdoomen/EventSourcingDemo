namespace ClubAdmin.Shared.Domain;

/// <summary>
/// Value object representing a monetary amount in a specific currency.
/// </summary>
public readonly record struct Money(decimal Amount, string CurrencyCode)
{
    public static Money Zero(string currencyCode = "EUR") => new(0m, currencyCode);

    public static Money Of(decimal amount, string currencyCode = "EUR")
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Monetary amounts cannot be negative.");
        }

        return new Money(amount, currencyCode);
    }

    public Money Add(Money other)
    {
        GuardSameCurrency(other);
        return new Money(Amount + other.Amount, CurrencyCode);
    }

    public Money Subtract(Money other)
    {
        GuardSameCurrency(other);
        return new Money(Amount - other.Amount, CurrencyCode);
    }

    public Money Percentage(decimal percent) =>
        new(Math.Round(Amount * percent / 100m, 2, MidpointRounding.AwayFromZero), CurrencyCode);

    public override string ToString() => $"{Amount:F2} {CurrencyCode}";

    private void GuardSameCurrency(Money other)
    {
        if (CurrencyCode != other.CurrencyCode)
        {
            throw new InvalidOperationException(
                $"Cannot operate on different currencies: {CurrencyCode} vs {other.CurrencyCode}.");
        }
    }
}
