using ClubAdmin.Finances.Domain;
using FluentAssertions;
using Xunit;

namespace ClubAdmin.Finances.Specs;

public class BudgetAllocationSpecs
{
    [Fact]
    public void Defining_a_budget_creates_plan_with_correct_amounts()
    {
        var (plan, evt) = BudgetPlan.Define(2025, BookingCode.FieldRental, 5000m);

        evt.Year.Should().Be(2025);
        evt.BookingCode.Should().Be(BookingCode.FieldRental.ToString());
        evt.BudgetAmount.Should().Be(5000m);

        plan.Budgets[BookingCode.FieldRental].Should().Be(5000m);
    }

    [Fact]
    public void Allocating_budget_increases_allocated_amount()
    {
        var (plan, _) = BudgetPlan.Define(2025, BookingCode.Equipment, 2000m);

        plan.Allocate(BookingCode.Equipment, 500m, "EUR", "Winter training equipment");

        plan.Budgets[BookingCode.Equipment].Should().Be(2500m);
    }

    [Fact]
    public void Negative_budget_amount_throws()
    {
        var act = () => BudgetPlan.Define(2025, BookingCode.Administration, -100m);
        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("amount");
    }
}
