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
        OnNewAsync<RegisterMember>(
            cmd => new MemberId(cmd.MemberId),
            (member, cmd, _) =>
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

        OnExistingAsync<UpdateMemberProfile>(
            cmd => new MemberId(cmd.MemberId),
            (member, cmd, _) =>
            {
                member.UpdateProfile(cmd.FirstName, cmd.LastName, cmd.DateOfBirth);
                return Task.CompletedTask;
            });

        OnExistingAsync<TerminateMembership>(
            cmd => new MemberId(cmd.MemberId),
            (member, cmd, _) =>
            {
                member.Terminate(cmd.Reason);
                return Task.CompletedTask;
            });

        OnExistingAsync<AssignContributionCategory>(
            cmd => new MemberId(cmd.MemberId),
            (member, cmd, _) =>
            {
                member.AssignContributionCategory(
                    Enum.Parse<Domain.ContributionCategory>(cmd.Category),
                    Enum.Parse<Domain.PaymentFrequency>(cmd.PaymentFrequency),
                    Money.Of(cmd.ContributionAmount, cmd.CurrencyCode),
                    cmd.PartialPercentage);

                return Task.CompletedTask;
            });

        OnExistingAsync<UpdateBankingDetails>(
            cmd => new MemberId(cmd.MemberId),
            (member, cmd, _) =>
            {
                member.UpdateBankingDetails(Domain.BankingDetails.Create(cmd.Iban, cmd.AccountHolder));
                return Task.CompletedTask;
            });
    }
}
