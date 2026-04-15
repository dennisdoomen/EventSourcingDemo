using ClubAdmin.Finances.Domain.Events;

namespace ClubAdmin.Finances.Domain;

/// <summary>
/// Yearly financial plan that tracks budget allocations per booking code.
/// </summary>
public class BudgetPlan
{
    private readonly Dictionary<BookingCode, decimal> _budgets = new();

    public BudgetPlanId Id { get; private set; }

    public IReadOnlyDictionary<BookingCode, decimal> Budgets => _budgets;

    public static (BudgetPlan, BudgetDefined) Define(
        int year,
        BookingCode bookingCode,
        decimal amount,
        string currencyCode = "EUR")
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Budget amount cannot be negative.");
        }

        var plan = new BudgetPlan();
        var evt = new BudgetDefined(year, bookingCode.ToString(), amount, currencyCode);
        plan.Apply(evt);
        return (plan, evt);
    }

    public BudgetAllocated Allocate(BookingCode bookingCode, decimal amount, string currencyCode, string reason)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Allocation amount cannot be negative.");
        }

        var evt = new BudgetAllocated(Id.Year, bookingCode.ToString(), amount, currencyCode, reason);
        Apply(evt);
        return evt;
    }

    private void Apply(BudgetDefined evt)
    {
        Id = BudgetPlanId.For(evt.Year);
        _budgets[Enum.Parse<BookingCode>(evt.BookingCode)] = evt.BudgetAmount;
    }

    private void Apply(BudgetAllocated evt)
    {
        var code = Enum.Parse<BookingCode>(evt.BookingCode);
        _budgets[code] = _budgets.GetValueOrDefault(code) + evt.AllocatedAmount;
    }
}
