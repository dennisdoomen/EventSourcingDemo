using ClubAdmin.Shared.Domain;

namespace ClubAdmin.Members.Domain;

/// <summary>
/// Value object encapsulating the banking details required for invoicing.
/// </summary>
public record BankingDetails
{
    public string Iban { get; init; }

    public string AccountHolder { get; init; }

    private BankingDetails(string iban, string accountHolder)
    {
        Iban = iban;
        AccountHolder = accountHolder;
    }

    public static BankingDetails Create(string iban, string accountHolder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(iban, nameof(iban));
        ArgumentException.ThrowIfNullOrWhiteSpace(accountHolder, nameof(accountHolder));

        string normalizedIban = iban.Replace(" ", "").ToUpperInvariant();
        if (!IsValidIban(normalizedIban))
        {
            throw new ArgumentException($"'{iban}' is not a valid IBAN.", nameof(iban));
        }

        return new BankingDetails(normalizedIban, accountHolder.Trim());
    }

    private static bool IsValidIban(string iban)
    {
        if (iban.Length < 15 || iban.Length > 34)
        {
            return false;
        }

        // Basic character check: 2 alpha + 2 digit + alphanumeric remainder
        if (!char.IsLetter(iban[0]) || !char.IsLetter(iban[1]) ||
            !char.IsDigit(iban[2]) || !char.IsDigit(iban[3]))
        {
            return false;
        }

        // MOD-97 check
        string rearranged = iban[4..] + iban[..4];
        string numericString = string.Concat(rearranged.Select(c => char.IsLetter(c) ? (c - 'A' + 10).ToString() : c.ToString()));

        int remainder = numericString.Aggregate(0, (current, digit) => (current * 10 + (digit - '0')) % 97);
        return remainder == 1;
    }
}
