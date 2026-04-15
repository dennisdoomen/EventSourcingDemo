using ClubAdmin.Members.Domain;
using ClubAdmin.Members.Domain.Events;
using ClubAdmin.Shared.Domain;
using FluentAssertions;
using Xunit;

namespace ClubAdmin.Members.Specs;

public class MemberRegistrationSpecs
{
    [Fact]
    public void Registering_a_new_member_raises_MemberRegistered()
    {
        // Arrange
        var member = new Member();
        var memberId = new MemberId(Guid.NewGuid().ToString());

        // Act
        member.Register(
            memberId,
            clubId: "club-1",
            firstName: "Jane",
            lastName: "Doe",
            dateOfBirth: new DateOnly(1990, 6, 15),
            membershipType: MembershipType.Regular);

        // Assert
        var events = member.Changes.Select(c => c.Payload).ToList();
        events.Should().ContainSingle().Which.Should().BeOfType<MemberRegistered>();

        var registered = (MemberRegistered)events[0];
        registered.MemberId.Should().Be(memberId.Value);
        registered.FirstName.Should().Be("Jane");
        registered.LastName.Should().Be("Doe");
        registered.MembershipType.Should().Be(MembershipType.Regular.ToString());
    }

    [Fact]
    public void Registering_a_sponsor_member_records_correct_membership_type()
    {
        var member = new Member();

        member.Register(
            new MemberId(Guid.NewGuid().ToString()),
            "club-1",
            "ACME",
            "Corp",
            new DateOnly(1980, 1, 1),
            MembershipType.Sponsor);

        var evt = member.Changes.Select(c => c.Payload).OfType<MemberRegistered>().Single();
        evt.MembershipType.Should().Be(MembershipType.Sponsor.ToString());
    }

    [Fact]
    public void Registering_a_member_without_first_name_throws()
    {
        var member = new Member();

        var act = () => member.Register(
            new MemberId(Guid.NewGuid().ToString()),
            "club-1",
            firstName: "",
            lastName: "Doe",
            new DateOnly(1990, 1, 1),
            MembershipType.Regular);

        act.Should().Throw<ArgumentException>().WithParameterName("firstName");
    }
}
