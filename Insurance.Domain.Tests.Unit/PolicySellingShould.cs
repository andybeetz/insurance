namespace Insurance.Domain.Tests.Unit;

public static class PolicySellingShould
{
    public static IEnumerable<TestCaseData> SellCases()
    {
        Resulting<Policy> SellHousehold(
            PolicyPeriod period,
            Money amount,
            bool hasClaims,
            bool autoRenew,
            PolicyHolders holders,
            InsuredProperty property,
            IReadOnlyCollection<PolicyPayment> payments)
        {
            var r = HouseholdPolicy.Sell(period, amount, hasClaims, autoRenew, holders, property, payments);
            return r.IsSuccess
                ? Resulting<Policy>.Success(r.Value)
                : Resulting<Policy>.Failure(r.Error!);
        }

        Resulting<Policy> SellBuyToLet(
            PolicyPeriod period,
            Money amount,
            bool hasClaims,
            bool autoRenew,
            PolicyHolders holders,
            InsuredProperty property,
            IReadOnlyCollection<PolicyPayment> payments)
        {
            var r = BuyToLetPolicy.Sell(period, amount, hasClaims, autoRenew, holders, property, payments);
            return r.IsSuccess
                ? Resulting<Policy>.Success(r.Value)
                : Resulting<Policy>.Failure(r.Error!);
        }

        yield return new TestCaseData(
                (Func<PolicyPeriod, Money, bool, bool, PolicyHolders, InsuredProperty, IReadOnlyCollection<PolicyPayment>, Resulting<Policy>>)SellHousehold)
            .SetName("Sell should create a HouseholdPolicy with a UniqueReference");

        yield return new TestCaseData(
                (Func<PolicyPeriod, Money, bool, bool, PolicyHolders, InsuredProperty, IReadOnlyCollection<PolicyPayment>, Resulting<Policy>>)SellBuyToLet)
            .SetName("Sell should create a BuyToLetPolicy with a UniqueReference");
    }
    
    public static IEnumerable<TestCaseData> SixtyDayCases()
    {
        Resulting<Policy> SellHousehold(
            PolicyPeriod period,
            Money amount,
            bool hasClaims,
            bool autoRenew,
            PolicyHolders holders,
            InsuredProperty property,
            IReadOnlyCollection<PolicyPayment> payments)
        {
            var r = HouseholdPolicy.Sell(period, amount, hasClaims, autoRenew, holders, property, payments);
            return r.IsSuccess
                ? Resulting<Policy>.Success(r.Value)
                : Resulting<Policy>.Failure(r.Error!);
        }

        Resulting<Policy> SellBuyToLet(
            PolicyPeriod period,
            Money amount,
            bool hasClaims,
            bool autoRenew,
            PolicyHolders holders,
            InsuredProperty property,
            IReadOnlyCollection<PolicyPayment> payments)
        {
            var r = BuyToLetPolicy.Sell(period, amount, hasClaims, autoRenew, holders, property, payments);
            return r.IsSuccess
                ? Resulting<Policy>.Success(r.Value)
                : Resulting<Policy>.Failure(r.Error!);
        }

        yield return new TestCaseData(
                (Func<PolicyPeriod, Money, bool, bool, PolicyHolders, InsuredProperty, IReadOnlyCollection<PolicyPayment>, Resulting<Policy>>)SellHousehold)
            .SetName("Sell should not create a HouseholdPolicy more than 60 days in advance");

        yield return new TestCaseData(
                (Func<PolicyPeriod, Money, bool, bool, PolicyHolders, InsuredProperty, IReadOnlyCollection<PolicyPayment>, Resulting<Policy>>)SellBuyToLet)
            .SetName("Sell should not create a BuyToLetPolicy more than 60 days in advance");
    }

    [TestCaseSource(nameof(SellCases))]
    public static void SellANewPolicyWithUniqueReference(
        Func<PolicyPeriod, Money, bool, bool, PolicyHolders, InsuredProperty, IReadOnlyCollection<PolicyPayment>, Resulting<Policy>> sell)
    {
        var period = PolicyPeriod.Create(
            startDate: DateOnly.FromDateTime(DateTime.UtcNow.Date),
            endDate: DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(365))).Value;

        var amount = Money.Create(0m).Value;

        var holder = PolicyHolder.Create(
            firstName: "Test",
            lastName: "User",
            dateOfBirth: DateOnly.FromDateTime(DateTime.UtcNow.Date.AddYears(-30))).Value;

        var property = InsuredProperty.Create(
            addressLine1: "1 Test Street",
            addressLine2: null,
            addressLine3: null,
            postCode: "ZZ1 1ZZ").Value;

        var result = sell(period, amount, false, false, PolicyHolders.Create([holder], period.StartDate).Value, property, Array.Empty<PolicyPayment>());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.UniqueReference, Is.Not.EqualTo(Guid.Empty));
        }
    }
    
    [TestCaseSource(nameof(SixtyDayCases))]
    public static void CannotSellANewPolicyMoreThanSixtyDaysInAdvance(
        Func<PolicyPeriod, Money, bool, bool, PolicyHolders, InsuredProperty, IReadOnlyCollection<PolicyPayment>, Resulting<Policy>> sell)
    {
        var period = PolicyPeriod.Create(
            startDate: DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(61)),
            endDate: DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(365 + 61))).Value;

        var amount = Money.Create(10m).Value;

        var holder = PolicyHolder.Create(
            firstName: "Test",
            lastName: "User",
            dateOfBirth: DateOnly.FromDateTime(DateTime.UtcNow.Date.AddYears(-30))).Value;

        var property = InsuredProperty.Create(
            addressLine1: "1 Test Street",
            addressLine2: null,
            addressLine3: null,
            postCode: "ZZ1 1ZZ").Value;

        var result = sell(period, amount, false, false, PolicyHolders.Create([holder], period.StartDate).Value, property, Array.Empty<PolicyPayment>());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error!.Code, Is.EqualTo("policy.startDate.tooFarInAdvance"));
            Assert.That(result.Error!.Description, Is.Not.Null.Or.Empty);
        }
    }
}