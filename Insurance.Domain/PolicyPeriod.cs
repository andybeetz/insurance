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

        return Resulting<PolicyPeriod>.Success(new PolicyPeriod(startDate, endDate));
    }
}