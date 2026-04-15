namespace ClubAdmin.Members.Domain;

/// <summary>
/// How often a member pays their contribution fee during the billing cycle.
/// The billing cycle spans 10 months (August through May/June).
/// </summary>
public enum PaymentFrequency
{
    Monthly,
    Quarterly,
    FullSeason
}
