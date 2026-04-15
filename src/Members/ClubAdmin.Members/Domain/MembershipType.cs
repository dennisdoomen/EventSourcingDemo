namespace ClubAdmin.Members.Domain;

/// <summary>
/// Distinguishes regular playing members from financial sponsors.
/// Sponsors have a separate invoicing flow.
/// </summary>
public enum MembershipType
{
    Regular,
    Sponsor
}
