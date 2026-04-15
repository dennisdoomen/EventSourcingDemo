namespace ClubAdmin.Finances.Domain;

/// <summary>
/// Strongly-typed identifier for a yearly budget plan.
/// </summary>
public readonly record struct BudgetPlanId(int Year)
{
    public static BudgetPlanId For(int year) => new(year);

    public override string ToString() => $"budget-{Year}";
}
