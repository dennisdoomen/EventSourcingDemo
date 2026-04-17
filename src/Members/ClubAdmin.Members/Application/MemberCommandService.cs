using ClubAdmin.Members.Application.Commands;
using ClubAdmin.Members.Domain;
using ClubAdmin.Shared.Domain;
using Eventuous;

namespace ClubAdmin.Members.Application;

/// <summary>
/// Handles member commands by delegating to the <see cref="Member"/> aggregate.
/// </summary>
public class MemberCommandService : CommandService<Member, MemberState, MemberId>
{
    public MemberCommandService(IEventStore store) : base(store)
    {
        On<RegisterMember>()
            .InState(ExpectedState.New)
            .GetId(cmd => new MemberId(cmd.MemberId))
            .ActAsync((member, cmd, _) =>
            {
                member.Register(
                    new MemberId(cmd.MemberId),
                    cmd.ClubId,
                    cmd.FirstName,
                    cmd.LastName,
                    cmd.DateOfBirth,
                    Enum.Parse<MembershipType>(cmd.MembershipType));

                return Task.CompletedTask;
            });

        On<UpdateMemberProfile>()
            .InState(ExpectedState.Existing)
            .GetId(cmd => new MemberId(cmd.MemberId))
            .ActAsync((member, cmd, _) =>
            {
                member.UpdateProfile(cmd.FirstName, cmd.LastName, cmd.DateOfBirth);
                return Task.CompletedTask;
            });

        On<TerminateMembership>()
            .InState(ExpectedState.Existing)
            .GetId(cmd => new MemberId(cmd.MemberId))
            .ActAsync((member, cmd, _) =>
            {
                member.Terminate(cmd.Reason);
                return Task.CompletedTask;
            });

        On<AssignContributionCategory>()
            .InState(ExpectedState.Existing)
            .GetId(cmd => new MemberId(cmd.MemberId))
            .ActAsync((member, cmd, _) =>
            {
                member.AssignContributionCategory(
                    Enum.Parse<Domain.ContributionCategory>(cmd.Category),
                    Enum.Parse<Domain.PaymentFrequency>(cmd.PaymentFrequency),
                    Money.Of(cmd.ContributionAmount, cmd.CurrencyCode),
                    cmd.PartialPercentage);

                return Task.CompletedTask;
            });

        On<UpdateBankingDetails>()
            .InState(ExpectedState.Existing)
            .GetId(cmd => new MemberId(cmd.MemberId))
            .ActAsync((member, cmd, _) =>
            {
                member.UpdateBankingDetails(Domain.BankingDetails.Create(cmd.Iban, cmd.AccountHolder));
                return Task.CompletedTask;
            });
    }
}
