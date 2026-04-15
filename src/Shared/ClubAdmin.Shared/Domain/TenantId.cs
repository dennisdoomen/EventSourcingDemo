namespace ClubAdmin.Shared.Domain;

/// <summary>
/// Multi-tenant discriminator — one per sports club deployment.
/// </summary>
public readonly record struct TenantId(string Value)
{
    public static TenantId Parse(string value) => new(value);

    public override string ToString() => Value;
}
