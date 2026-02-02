namespace Insurance.Domain.Tests.Unit;

[TestFixture]
public class BuyToLetPolicyShould
{
    [Test]
    public void SellANewBuyToLetPolicy()
    {
        var policyHolderResult = PolicyHolder.Create(
            firstName: "Test",
            lastName: "User",
            dateOfBirth: DateOnly.FromDateTime(DateTime.UtcNow.Date.AddYears(-30)));

        var propertyResult = InsuredProperty.Create(
            addressLine1: "1 Test Street",
            addressLine2: null,
            addressLine3: null,
            postCode: "ZZ1 1ZZ");

        var policyPeriodResult = PolicyPeriod.Create(
            startDate: DateOnly.FromDateTime(DateTime.UtcNow.Date),
            endDate: DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(365)));

        Assume.That(policyHolderResult.IsSuccess, Is.True);
        Assume.That(propertyResult.IsSuccess, Is.True);

        var newPolicy = BuyToLetPolicy.Sell(
            period: policyPeriodResult.Value,
            amount: Money.Create(0m).Value,
            hasClaims: false,
            autoRenew: false,
            policyHolders: [policyHolderResult.Value],
            property: propertyResult.Value,
            payments: Array.Empty<PolicyPayment>());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(newPolicy.IsSuccess);
            Assert.That(newPolicy.Value.UniqueReference, Is.Not.EqualTo(Guid.Empty));
        }
    }
}