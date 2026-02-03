using Insurance.Application.Dtos.v1;
using Insurance.Domain;
using Insurance.Domain.Interfaces;

namespace Insurance.Application.Tests.Unit;

[TestFixture]
public class PolicyRenewerShould
{
    private sealed class TestClock(DateOnly today) : IAmAClock
    {
        public DateOnly Today { get; } = today;
    }

    [Test]
    public void RenewAHouseholdPolicy()
    {
        var policyStore = new PolicyStore();

        // Create a policy that is within the 30-day renewal window.
        var startDate = new DateOnly(2024, 01, 01);
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

        var initialPayment = PolicyPayment.Create(
            paymentReference: Guid.NewGuid(),
            paymentType: "CARD",
            amount: 50m).Value;

        var soldResult = HouseholdPolicy.Sell(
            period: period,
            amount: Money.Create(50m).Value,
            hasClaims: false,
            autoRenew: true,
            policyHolders: holders,
            property: property,
            payments: [initialPayment]);

        Assume.That(soldResult.IsSuccess, Is.True);

        var storedPolicy = soldResult.Value;
        var storeResult = policyStore.StoreHouseholdPolicy(storedPolicy);
        Assume.That(storeResult.IsSuccess, Is.True);

        var clock = new TestClock(today: storedPolicy.Period.EndDate.AddDays(-1));
        var sut = new PolicyRenewer(policyStore, clock);

        var renewalPaymentReference = Guid.NewGuid();

        var renewRequest = new HouseholdPolicyDto
        {
            UniqueReference = storedPolicy.UniqueReference,

            // These are not used to calculate renewal in the implementation (domain policy is the source of truth),
            // but they are required by the DTO.
            StartDate = storedPolicy.Period.StartDate,
            EndDate = storedPolicy.Period.EndDate,

            Amount = 77m,
            HasClaims = storedPolicy.HasClaims,
            AutoRenew = storedPolicy.AutoRenew,
            PolicyHolders =
            [
                new PolicyHolderDto
                {
                    FirstName = holder.FirstName,
                    LastName = holder.LastName,
                    DateOfBirth = holder.DateOfBirth
                }
            ],
            Property = new PropertyDto
            {
                AddressLine1 = property.AddressLine1,
                AddressLine2 = property.AddressLine2,
                AddressLine3 = property.AddressLine3,
                PostCode = property.PostCode.Value
            },
            Payments =
            [
                new PaymentDto
                {
                    PaymentReference = renewalPaymentReference,
                    PaymentType = "CARD",
                    Amount = 77m
                }
            ]
        };

        var result = sut.RenewHouseholdPolicy(renewRequest);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);

            var renewed = result.Value;

            Assert.That(renewed.UniqueReference, Is.EqualTo(storedPolicy.UniqueReference));

            // Period should roll forward by 1 year: new start == old end
            Assert.That(renewed.StartDate, Is.EqualTo(period.EndDate));
            Assert.That(renewed.EndDate, Is.EqualTo(period.EndDate.AddYears(1)));

            // Renewal updates amount
            Assert.That(renewed.Amount, Is.EqualTo(77m));

            // Renewal adds a payment (domain appends it)
            Assert.That(renewed.Payments, Has.Count.EqualTo(2));
            Assert.That(renewed.Payments.Last().PaymentReference, Is.EqualTo(renewalPaymentReference));
            Assert.That(renewed.Payments.Last().PaymentType, Is.EqualTo("CARD"));
            Assert.That(renewed.Payments.Last().Amount, Is.EqualTo(77m));
        }
    }

    [Test]
    public void FailToRenewAHouseholdPolicyWithMultiplePayments()
    {
        var policyStore = new PolicyStore();

        // Create a policy that is within the 30-day renewal window.
        var startDate = new DateOnly(2024, 01, 01);
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

        var initialPayment = PolicyPayment.Create(
            paymentReference: Guid.NewGuid(),
            paymentType: "CARD",
            amount: 50m).Value;

        var soldResult = HouseholdPolicy.Sell(
            period: period,
            amount: Money.Create(50m).Value,
            hasClaims: false,
            autoRenew: true,
            policyHolders: holders,
            property: property,
            payments: [initialPayment]);

        Assume.That(soldResult.IsSuccess, Is.True);

        var storedPolicy = soldResult.Value;
        var storeResult = policyStore.StoreHouseholdPolicy(storedPolicy);
        Assume.That(storeResult.IsSuccess, Is.True);

        var clock = new TestClock(today: storedPolicy.Period.EndDate.AddDays(-1));
        var sut = new PolicyRenewer(policyStore, clock);

        var renewalPaymentReference = Guid.NewGuid();

        var renewRequest = new HouseholdPolicyDto
        {
            UniqueReference = storedPolicy.UniqueReference,

            // These are not used to calculate renewal in the implementation (domain policy is the source of truth),
            // but they are required by the DTO.
            StartDate = storedPolicy.Period.StartDate,
            EndDate = storedPolicy.Period.EndDate,

            Amount = 77m,
            HasClaims = storedPolicy.HasClaims,
            AutoRenew = storedPolicy.AutoRenew,
            PolicyHolders =
            [
                new PolicyHolderDto
                {
                    FirstName = holder.FirstName,
                    LastName = holder.LastName,
                    DateOfBirth = holder.DateOfBirth
                }
            ],
            Property = new PropertyDto
            {
                AddressLine1 = property.AddressLine1,
                AddressLine2 = property.AddressLine2,
                AddressLine3 = property.AddressLine3,
                PostCode = property.PostCode.Value
            },
            Payments =
            [
                new PaymentDto
                {
                    PaymentReference = renewalPaymentReference,
                    PaymentType = "CARD",
                    Amount = 77m
                },
                new PaymentDto
                {
                    PaymentReference = Guid.NewGuid(),
                    PaymentType = "CARD",
                    Amount = 55m
                }
            ]
        };

        var result = sut.RenewHouseholdPolicy(renewRequest);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error!.Code, Is.EqualTo("policy.renewal.payment.tooMany"));
            Assert.That(result.Error.Description, Is.Not.Null.Or.Empty);
        }
    }
}