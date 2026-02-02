namespace Insurance.Domain;

public sealed record PostCode
{
    private PostCode(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Resulting<PostCode> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("postcode.required", "PostCode is required.");

        var normalized = value.Trim().ToUpperInvariant();

        // Keep the rule you had before (<= 8). You can replace with stricter country-specific validation later.
        if (normalized.Length > 8)
            return Error.Validation("postcode.invalid", "PostCode must be 8 characters or fewer.");

        return Resulting<PostCode>.Success(new PostCode(normalized));
    }

    public override string ToString() => Value;
}