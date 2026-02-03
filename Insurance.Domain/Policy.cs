using Insurance.Domain.Interfaces;

namespace Insurance.Domain;

public abstract class Policy
{
    protected Policy(
        Guid uniqueReference,
        PolicyPeriod period,
        Money amount,
        bool hasClaims,
        bool autoRenew,
        PolicyHolders policyHolders,
        InsuredProperty property,
        IReadOnlyCollection<PolicyPayment> payments)
    {
        UniqueReference = uniqueReference;
        Period = period;
        Amount = amount;
        HasClaims = hasClaims;
        AutoRenew = autoRenew;
        PolicyHolders = policyHolders;
        Property = property;
        Payments = payments;
    }

    public Guid UniqueReference { get; private set; }
    public PolicyPeriod Period { get; private set; }
    public Money Amount { get; private set; }
    public bool HasClaims { get; private set; }
    public bool AutoRenew { get; private set; }
    public PolicyHolders PolicyHolders { get; private set; }
    public InsuredProperty Property { get; private set; }
    public IReadOnlyCollection<PolicyPayment> Payments { get; private set; }

    public Result Renew(
        Money newAmount,
        PolicyPayment renewalPayment,
        IAmAClock? clock = null)
    {
        clock ??= new SystemClock();

        var today = clock.Today;
        var renewalWindowStart = Period.EndDate.AddDays(-30);

        if (today < renewalWindowStart)
            return Error.Validation(
                "policy.renewal.tooEarly",
                "Policy can only be renewed within 30 days of the end date.");

        if (today > Period.EndDate)
            return Error.Validation(
                "policy.renewal.expired",
                "Policy cannot be renewed after the end date.");

        var newStart = Period.EndDate;
        var newEnd = newStart.AddYears(1);

        var newPeriodResult = PolicyPeriod.Create(newStart, newEnd);
        if (!newPeriodResult.IsSuccess)
            return newPeriodResult.Error!;

        var updatedPayments = Payments.ToList();

        // The language around the feature (Renewing a policy) differed somewhat from the 
        // renewal requirements. Renew a policy vs Adding a Renewal to a policy.
        // I've gone with renewing a policy and that a payment is always required.
        // My design also assumes that the payment system is completely separated
        
        if (renewalPayment is null)
        {
            return Error.Validation(
                "policy.renewal.payment.required",
                "A renewal payment must be provided when AutoRenew is enabled.");
        }

        if (renewalPayment.Amount != newAmount)
        {
            return Error.Validation(
                "policy.renewal.payment.amount.mismatch",
                "Renewal payment amount must match the renewed policy amount.");
        }

        updatedPayments.Add(renewalPayment);

        Period = newPeriodResult.Value;
        Amount = newAmount;
        Payments = updatedPayments;

        return Result.Success();
    }
}