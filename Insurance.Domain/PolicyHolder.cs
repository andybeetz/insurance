namespace Insurance.Domain;

public sealed record PolicyHolder
{
    private PolicyHolder(string firstName, string lastName, DateOnly dateOfBirth)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
    }

    public string FirstName { get; }
    public string LastName { get; }
    public DateOnly DateOfBirth { get; }

    public static Resulting<PolicyHolder> Create(string firstName, string lastName, DateOnly dateOfBirth)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Error.Validation("policyHolder.firstName.required", "First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            return Error.Validation("policyHolder.lastName.required", "Last name is required.");

        // Example rule (tune to your business): must be in the past
        if (dateOfBirth >= DateOnly.FromDateTime(DateTime.UtcNow.Date))
            return Error.Validation("policyHolder.dob.invalid", "Date of birth must be in the past.");

        return Resulting<PolicyHolder>.Success(new PolicyHolder(firstName.Trim(), lastName.Trim(), dateOfBirth));
    }
}