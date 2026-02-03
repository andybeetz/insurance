namespace Insurance.Domain.Tests.Unit;

[TestFixture]
public class PolicyRenewalShould
{
    private sealed class TestClock(DateOnly today) : Interfaces.IAmAClock
    {
        public DateOnly Today { get; } = today;
    }

    [Test]
    public void NotRenewMoreThanThirtyDaysBeforeEndDate()
    {
        var policy = CreateHouseholdPolicy(
            startDate: new DateOnly(2024, 01, 01),
            autoRenew: false,
            payments: []);

        var clock = new TestClock(today: policy.Period.EndDate.AddDays(-31));
        
        var newAmount = Money.Create(123m).Value;
        
        var renewalPayment = PolicyPayment.Create(
            paymentReference: Guid.NewGuid(),
            paymentType: "DIRECTDEBIT",
            amount: newAmount.Amount).Value;

        var result = policy.Renew(newAmount, renewalPayment, clock: clock);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error!.Code, Is.EqualTo("policy.renewal.tooEarly"));
            Assert.That(result.Error.Description, Is.Not.Null.Or.Empty);
        }
    }

    [Test]
    public void NotRenewAfterEndDate()
    {
        var policy = CreateHouseholdPolicy(
            startDate: new DateOnly(2024, 01, 01),
            autoRenew: false,
            payments: []);

        var clock = new TestClock(today: policy.Period.EndDate.AddDays(1));

        var newAmount = Money.Create(123m).Value;
        
        var renewalPayment = PolicyPayment.Create(
            paymentReference: Guid.NewGuid(),
            paymentType: "DIRECTDEBIT",
            amount: newAmount.Amount).Value;

        var result = policy.Renew(newAmount, renewalPayment, clock: clock);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error!.Code, Is.EqualTo("policy.renewal.expired"));
            Assert.That(result.Error.Description, Is.Not.Null.Or.Empty);
        }
    }

    [Test]
    public void UpdatePeriodAndAmountOnSuccessfulRenewal()
    {
        var policy = CreateHouseholdPolicy(
            startDate: new DateOnly(2024, 01, 01),
            autoRenew: false,
            payments: []);

        var oldEnd = policy.Period.EndDate;
        var clock = new TestClock(today: oldEnd.AddDays(-30));

        var newAmount = Money.Create(999.99m).Value;
        
        var renewalPayment = PolicyPayment.Create(
            paymentReference: Guid.NewGuid(),
            paymentType: "CARD",
            amount: newAmount.Amount).Value;

        var result = policy.Renew(newAmount, renewalPayment, clock: clock);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(policy.Period.StartDate, Is.EqualTo(oldEnd));
            Assert.That(policy.Period.EndDate, Is.EqualTo(oldEnd.AddYears(1)));
            Assert.That(policy.Amount, Is.EqualTo(newAmount));
        }
    }

    [Test]
    public void AddAPaymentWhenAutoRenewIsTrue()
    {
        var initialPayment = PolicyPayment.Create(Guid.NewGuid(), "CARD", 50m).Value;

        var policy = CreateHouseholdPolicy(
            startDate: new DateOnly(2024, 01, 01),
            autoRenew: true,
            payments: [initialPayment]);

        var clock = new TestClock(today: policy.Period.EndDate.AddDays(-1));
        var newAmount = Money.Create(77m).Value;

        var renewalPayment = PolicyPayment.Create(
            paymentReference: Guid.NewGuid(),
            paymentType: "CARD",
            amount: newAmount.Amount).Value;

        var beforeCount = policy.Payments.Count;

        var result = policy.Renew(newAmount, renewalPayment: renewalPayment, clock: clock);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(policy.Payments.Count, Is.EqualTo(beforeCount + 1));
            Assert.That(policy.Payments.Last(), Is.EqualTo(renewalPayment));
        }
    }
    
    [Test]
    public void NotRenewAPolicyIfPaymentIsNull()
    {
        var initialPayment = PolicyPayment.Create(Guid.NewGuid(), "CARD", 50m).Value;
    
        var policy = CreateHouseholdPolicy(
            startDate: new DateOnly(2024, 01, 01),
            autoRenew: false,
            payments: [initialPayment]);
    
        var clock = new TestClock(today: policy.Period.EndDate.AddDays(-1));
        var newAmount = Money.Create(77m).Value;
    
        var beforeCount = policy.Payments.Count;
    
        var result = policy.Renew(newAmount, null!, clock: clock);
    
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error!.Code, Is.EqualTo("policy.renewal.payment.required"));
            Assert.That(result.Error.Description, Is.Not.Null.Or.Empty);
        }
    }

    // [Ignore("This was for requirements that did not fit")]
    // [Test]
    // public void NotAddAPaymentWhenAutoRenewIsFalse()
    // {
    //     var initialPayment = PolicyPayment.Create(Guid.NewGuid(), "CARD", 50m).Value;
    //
    //     var policy = CreateHouseholdPolicy(
    //         startDate: new DateOnly(2024, 01, 01),
    //         autoRenew: false,
    //         payments: [initialPayment]);
    //
    //     var clock = new TestClock(today: policy.Period.EndDate.AddDays(-1));
    //     var newAmount = Money.Create(77m).Value;
    //
    //     var beforeCount = policy.Payments.Count;
    //
    //     var result = policy.Renew(newAmount, clock: clock);
    //
    //     using (Assert.EnterMultipleScope())
    //     {
    //         Assert.That(result.IsSuccess, Is.True);
    //         Assert.That(policy.Payments, Has.Count.EqualTo(beforeCount));
    //     }
    // }

    private static HouseholdPolicy CreateHouseholdPolicy(
        DateOnly startDate,
        bool autoRenew,
        IReadOnlyCollection<PolicyPayment> payments)
    {
        var period = PolicyPeriod.Create(startDate, startDate.AddYears(1)).Value;

        var holder = PolicyHolder.Create(
            firstName: "Test",
            lastName: "User",
            dateOfBirth: new DateOnly(1990, 01, 01)).Value;

        var holders = PolicyHolders.Create([holder], period.StartDate).Value;

        var property = InsuredProperty.Create(
            addressLine1: "1 Test Street",
            addressLine2: null,
            addressLine3: null,
            postCode: "ZZ1 1ZZ").Value;

        var result = HouseholdPolicy.Sell(
            period: period,
            amount: Money.Create(50m).Value,
            hasClaims: false,
            autoRenew: autoRenew,
            policyHolders: holders,
            property: property,
            payments: payments);

        Assume.That(result.IsSuccess, Is.True);
        return result.Value;
    }
}