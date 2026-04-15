using ClubAdmin.Members.Domain;
using ClubAdmin.Members.Domain.Events;
using ClubAdmin.Shared.Domain;
using FluentAssertions;
using Xunit;

namespace ClubAdmin.Members.Specs;

public class ContributionAssignmentSpecs
{
    private static Member CreateRegisteredMember()
    {
        var member = new Member();
        member.Register(
            new MemberId(Guid.NewGuid().ToString()),
            "club-1",
            "Test",
            "Member",
            new DateOnly(2005, 3, 10),
            MembershipType.Regular);

        return member;
    }

    [Fact]
    public void Assigning_contribution_category_raises_ContributionCategoryAssigned()
    {
        var member = CreateRegisteredMember();

        member.AssignContributionCategory(
            ContributionCategory.Youth,
            PaymentFrequency.Monthly,
            Money.Of(25m));

        var evt = member.Changes.Select(c => c.Payload)
            .OfType<ContributionCategoryAssigned>()
            .Single();

        evt.Category.Should().Be(ContributionCategory.Youth.ToString());
        evt.PaymentFrequency.Should().Be(PaymentFrequency.Monthly.ToString());
        evt.ContributionAmount.Should().Be(25m);
        evt.PartialPercentage.Should().Be(100m);
    }

    [Fact]
    public void Late_joiner_receives_partial_contribution_percentage()
    {
        var member = CreateRegisteredMember();

        member.AssignContributionCategory(
            ContributionCategory.Senior,
            PaymentFrequency.Quarterly,
            Money.Of(200m),
            partialPercentage: 50m);

        var evt = member.Changes.Select(c => c.Payload)
            .OfType<ContributionCategoryAssigned>()
            .Single();

        evt.PartialPercentage.Should().Be(50m);
    }

    [Fact]
    public void Invalid_partial_percentage_throws()
    {
        var member = CreateRegisteredMember();

        var act = () => member.AssignContributionCategory(
            ContributionCategory.Junior,
            PaymentFrequency.FullSeason,
            Money.Of(150m),
            partialPercentage: 0m);

        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("partialPercentage");
    }
}
