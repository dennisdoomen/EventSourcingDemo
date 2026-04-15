namespace ClubAdmin.Members.Domain;

/// <summary>
/// Strongly-typed identifier for a club member.
/// </summary>
public record MemberId(string Value) : Id(Value);
