using ClubAdmin.Shared.Domain;

namespace ClubAdmin.Members.Domain.Events;

public record MemberRegistered(
    string MemberId,
    string ClubId,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string MembershipType,
    DateOnly RegisteredOn);

public record MemberProfileUpdated(
    string MemberId,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth);

public record MembershipTerminated(
    string MemberId,
    DateOnly EndDate,
    string Reason);

public record ContributionCategoryAssigned(
    string MemberId,
    string Category,
    string PaymentFrequency,
    decimal ContributionAmount,
    decimal PartialPercentage);

public record BankingDetailsUpdated(
    string MemberId,
    string Iban,
    string AccountHolder);
