using ClubAdmin.Members.Domain.Events;
using ClubAdmin.Shared.Domain;
using Eventuous;

namespace ClubAdmin.Members.Domain;

/// <summary>
/// Member aggregate — manages the full lifecycle of a sports club member.
/// </summary>
public class Member : Aggregate<MemberState>
{
    public void Register(
        MemberId memberId,
        string clubId,
        string firstName,
        string lastName,
        DateOnly dateOfBirth,
        MembershipType membershipType)
    {
        EnsureDoesntExist();

        ArgumentException.ThrowIfNullOrWhiteSpace(firstName, nameof(firstName));
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName, nameof(lastName));

        Apply(new MemberRegistered(
            memberId.Value,
            clubId,
            firstName,
            lastName,
            dateOfBirth,
            membershipType.ToString(),
            DateOnly.FromDateTime(DateTime.UtcNow)));
    }

    public void UpdateProfile(string firstName, string lastName, DateOnly dateOfBirth)
    {
        EnsureExists();
        EnsureActive();

        ArgumentException.ThrowIfNullOrWhiteSpace(firstName, nameof(firstName));
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName, nameof(lastName));

        Apply(new MemberProfileUpdated(State.Id!.Value, firstName, lastName, dateOfBirth));
    }

    public void Terminate(string reason)
    {
        EnsureExists();
        EnsureActive();

        Apply(new MembershipTerminated(
            State.Id!.Value,
            DateOnly.FromDateTime(DateTime.UtcNow),
            reason));
    }

    public void AssignContributionCategory(
        ContributionCategory category,
        PaymentFrequency frequency,
        Money contributionAmount,
        decimal partialPercentage = 100m)
    {
        EnsureExists();
        EnsureActive();

        if (partialPercentage is <= 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(partialPercentage),
                "Partial percentage must be between 1 and 100.");
        }

        Apply(new ContributionCategoryAssigned(
            State.Id!.Value,
            category.ToString(),
            frequency.ToString(),
            contributionAmount.Amount,
            partialPercentage));
    }

    public void UpdateBankingDetails(BankingDetails bankingDetails)
    {
        EnsureExists();
        EnsureActive();

        Apply(new BankingDetailsUpdated(
            State.Id!.Value,
            bankingDetails.Iban,
            bankingDetails.AccountHolder));
    }

    private void EnsureActive()
    {
        if (!State.IsActive)
        {
            throw new DomainException("Operation is not allowed on a terminated membership.");
        }
    }
}
