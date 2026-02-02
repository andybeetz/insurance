namespace Insurance.Domain;

public sealed record PolicyPeriod
{
    private PolicyPeriod(DateOnly startDate, DateOnly endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    public DateOnly StartDate { get; }
    public DateOnly EndDate { get; }

    public static Resulting<PolicyPeriod> Create(DateOnly startDate, DateOnly endDate)
    {
        if (startDate > endDate)
            return Error.Validation("policy.dates.invalid", "StartDate must be on or before EndDate.");
        
        // Period must be exactly one year in length
        var expectedEndDate = startDate.AddYears(1);
        if (endDate != expectedEndDate)
            return Error.Validation(
                "policy.period.invalidLength",
                "Policy period must be exactly 1 year in length.");

        return Resulting<PolicyPeriod>.Success(new PolicyPeriod(startDate, endDate));
    }
}