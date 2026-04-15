namespace ClubAdmin.Shared.Domain;

/// <summary>
/// Strongly-typed identifier for a sports club (tenant discriminator).
/// </summary>
public readonly record struct ClubId(Guid Value)
{
    public static ClubId New() => new(Guid.NewGuid());

    public static ClubId Parse(string value) => new(Guid.Parse(value));

    public override string ToString() => Value.ToString();
}
