using Insurance.Application.Interfaces;
using Insurance.Domain;

namespace Insurance.Application;

public static class SampleDataCreator
{
    public sealed record SeedReport(
        IReadOnlyCollection<Guid> HouseholdPolicyIds,
        IReadOnlyCollection<Guid> BuyToLetPolicyIds);

    public static SeedReport Seed(IStorePolicies policyStore)
    {
        var householdIds = new List<Guid>();
        var buyToLetIds = new List<Guid>();

        // Create household policies
        // expired
        householdIds.Add(StoreHousehold(policyStore,
            startDate: DateOnly.FromDateTime(DateTime.UtcNow.Date).AddYears(-2), // ended ~1 year ago
            amount: 120.00m,
            autoRenew: true,
            paymentType: "CARD"));

        // expiring in 15 days (=> within 30-day renewal window)
        householdIds.Add(StoreHousehold(policyStore,
            startDate: DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(15)).AddYears(-1),
            amount: 99.99m,
            autoRenew: true,
            paymentType: "DIRECTDEBIT"));

        // Create buy to let policies
        // expired
        buyToLetIds.Add(StoreBuyToLet(policyStore,
            startDate: DateOnly.FromDateTime(DateTime.UtcNow.Date).AddYears(-3), // ended ~2 years ago
            amount: 250.00m,
            autoRenew: true,
            paymentType: "CHEQUE"));

        // expiring in 15 days (=> within 30-day renewal window)
        buyToLetIds.Add(StoreBuyToLet(policyStore,
            startDate: DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(15)).AddYears(-1),
            amount: 180.50m,
            autoRenew: true,
            paymentType: "CARD"));

        return new SeedReport(householdIds, buyToLetIds);
    }

    private static Guid StoreHousehold(
        IStorePolicies policyStore,
        DateOnly startDate,
        decimal amount,
        bool autoRenew,
        string paymentType)
    {
        var period = PolicyPeriod.Create(startDate, startDate.AddYears(1)).Value;

        var holder = PolicyHolder.Create(
            firstName: "Sample",
            lastName: "Household",
            dateOfBirth: new DateOnly(1990, 01, 01)).Value;

        var holders = PolicyHolders.Create([holder], period.StartDate).Value;

        var property = InsuredProperty.Create(
            addressLine1: "1 Sample Street",
            addressLine2: null,
            addressLine3: null,
            postCode: "ZZ1 1ZZ").Value;

        var initialPayment = PolicyPayment.Create(
            paymentReference: Guid.NewGuid(),
            paymentType: paymentType,
            amount: amount).Value;

        var sold = HouseholdPolicy.Sell(
            period: period,
            amount: Money.Create(amount).Value,
            hasClaims: false,
            autoRenew: autoRenew,
            policyHolders: holders,
            property: property,
            payments: [initialPayment]);

        if (!sold.IsSuccess)
            return Guid.Empty; // keep seeding resilient; in a real app you’d log/throw

        policyStore.StoreHouseholdPolicy(sold.Value);
        return sold.Value.UniqueReference;
    }

    private static Guid StoreBuyToLet(
        IStorePolicies policyStore,
        DateOnly startDate,
        decimal amount,
        bool autoRenew,
        string paymentType)
    {
        var period = PolicyPeriod.Create(startDate, startDate.AddYears(1)).Value;

        var holder = PolicyHolder.Create(
            firstName: "Sample",
            lastName: "Landlord",
            dateOfBirth: new DateOnly(1985, 06, 15)).Value;

        var holders = PolicyHolders.Create([holder], period.StartDate).Value;

        var property = InsuredProperty.Create(
            addressLine1: "2 Sample Avenue",
            addressLine2: null,
            addressLine3: null,
            postCode: "YY1 2YY").Value;

        var initialPayment = PolicyPayment.Create(
            paymentReference: Guid.NewGuid(),
            paymentType: paymentType,
            amount: amount).Value;

        var sold = BuyToLetPolicy.Sell(
            period: period,
            amount: Money.Create(amount).Value,
            hasClaims: false,
            autoRenew: autoRenew,
            policyHolders: holders,
            property: property,
            payments: [initialPayment]);

        if (!sold.IsSuccess)
            return Guid.Empty; // keep seeding resilient; in a real app you’d log/throw

        policyStore.StoreBuyToLetPolicy(sold.Value);
        return sold.Value.UniqueReference;
    }
}