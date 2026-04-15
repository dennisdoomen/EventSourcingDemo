using ClubAdmin.Members.Domain.Events;
using Eventuous;

namespace ClubAdmin.Members.Domain;

/// <summary>
/// Immutable read-model of a Member aggregate, reconstructed from domain events.
/// </summary>
public record MemberState : State<MemberState>
{
    public MemberId? Id { get; init; }

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public DateOnly DateOfBirth { get; init; }

    public MembershipType MembershipType { get; init; }

    public ContributionCategory ContributionCategory { get; init; }

    public PaymentFrequency PaymentFrequency { get; init; }

    public decimal ContributionAmount { get; init; }

    public decimal PartialPercentage { get; init; } = 100m;

    public BankingDetails? BankingDetails { get; init; }

    public DateOnly? TerminatedOn { get; init; }

    public bool IsActive => TerminatedOn is null;

    public MemberState()
    {
        On<MemberRegistered>(Apply);
        On<MemberProfileUpdated>(Apply);
        On<MembershipTerminated>(Apply);
        On<ContributionCategoryAssigned>(Apply);
        On<BankingDetailsUpdated>(Apply);
    }

    private static MemberState Apply(MemberState state, MemberRegistered evt) =>
        state with
        {
            Id = new MemberId(evt.MemberId),
            FirstName = evt.FirstName,
            LastName = evt.LastName,
            DateOfBirth = evt.DateOfBirth,
            MembershipType = Enum.Parse<MembershipType>(evt.MembershipType)
        };

    private static MemberState Apply(MemberState state, MemberProfileUpdated evt) =>
        state with
        {
            FirstName = evt.FirstName,
            LastName = evt.LastName,
            DateOfBirth = evt.DateOfBirth
        };

    private static MemberState Apply(MemberState state, MembershipTerminated evt) =>
        state with { TerminatedOn = evt.EndDate };

    private static MemberState Apply(MemberState state, ContributionCategoryAssigned evt) =>
        state with
        {
            ContributionCategory = Enum.Parse<ContributionCategory>(evt.Category),
            PaymentFrequency = Enum.Parse<PaymentFrequency>(evt.PaymentFrequency),
            ContributionAmount = evt.ContributionAmount,
            PartialPercentage = evt.PartialPercentage
        };

    private static MemberState Apply(MemberState state, BankingDetailsUpdated evt) =>
        state with
        {
            BankingDetails = Domain.BankingDetails.Create(evt.Iban, evt.AccountHolder)
        };
}
