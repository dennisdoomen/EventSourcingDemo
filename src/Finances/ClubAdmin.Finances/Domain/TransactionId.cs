namespace ClubAdmin.Finances.Domain;

/// <summary>
/// Strongly-typed identifier for a financial transaction.
/// </summary>
public readonly record struct TransactionId(Guid Value)
{
    public static TransactionId New() => new(Guid.NewGuid());

    public static TransactionId Parse(string value) => new(Guid.Parse(value));

    public override string ToString() => Value.ToString();
}
