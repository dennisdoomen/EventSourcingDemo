using ClubAdmin.Finances.Application.Commands;
using ClubAdmin.Finances.Domain;
using ClubAdmin.Finances.Domain.Events;

namespace ClubAdmin.Finances.Application;

/// <summary>
/// Wolverine message handler for budget plan commands.
/// </summary>
public static class BudgetCommandHandler
{
    public static BudgetDefined Handle(DefineBudget cmd)
    {
        var bookingCode = Enum.Parse<BookingCode>(cmd.BookingCode);
        var (_, evt) = BudgetPlan.Define(cmd.Year, bookingCode, cmd.BudgetAmount, cmd.CurrencyCode);
        return evt;
    }

    public static BudgetAllocated Handle(AllocateBudget cmd, BudgetPlan plan)
    {
        var bookingCode = Enum.Parse<BookingCode>(cmd.BookingCode);
        return plan.Allocate(bookingCode, cmd.Amount, cmd.CurrencyCode, cmd.Reason);
    }
}
