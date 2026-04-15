using ClubAdmin.Shared.Domain;

namespace ClubAdmin.Members.Application.Commands;

public record RegisterMember(
    string MemberId,
    string ClubId,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string MembershipType);

public record UpdateMemberProfile(
    string MemberId,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth);

public record TerminateMembership(
    string MemberId,
    string Reason);

public record AssignContributionCategory(
    string MemberId,
    string Category,
    string PaymentFrequency,
    decimal ContributionAmount,
    string CurrencyCode = "EUR",
    decimal PartialPercentage = 100m);

public record UpdateBankingDetails(
    string MemberId,
    string Iban,
    string AccountHolder);
