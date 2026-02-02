namespace Insurance.Domain;

public sealed record Money
{
    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; }
    public string Currency { get; }

    public static Resulting<Money> Create(decimal amount, string currency = "GBP")
    {
        if (amount < 0)
            return Error.Validation("money.amount.invalid", "Amount must be zero or greater.");

        if (string.IsNullOrWhiteSpace(currency))
            return Error.Validation("money.currency.required", "Currency is required.");

        // Simple normalization; tune rules as needed
        currency = currency.Trim().ToUpperInvariant();

        // Use 2dp for “money”. If you need more nuance, consider currency-specific decimals.
        amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);

        return Resulting<Money>.Success(new Money(amount, currency));
    }
}