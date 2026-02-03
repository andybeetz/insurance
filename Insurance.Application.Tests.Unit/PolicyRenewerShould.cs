using Insurance.Application.Dtos.v1;
using Insurance.Domain;
using Insurance.Domain.Interfaces;

namespace Insurance.Application.Tests.Unit;

[TestFixture]
public class PolicyRenewerShould
{
    private const string PaymentType = "CARD";

    private sealed class TestClock(DateOnly today) : IAmAClock
    {
        public DateOnly Today { get; } = today;
    }

    private sealed record ArrangedPolicy(
        PolicyStore Store,
        PolicyPeriod Period,
        PolicyHolder Holder,
        InsuredProperty Property,
        HouseholdPolicy? Household,
        BuyToLetPolicy? BuyToLet)
    {
        public Guid UniqueReference =>
            Household?.UniqueReference ?? BuyToLet!.UniqueReference;

        public DateOnly ClockToday =>
            (Household?.Period.EndDate ?? BuyToLet!.Period.EndDate).AddDays(-1);

        public bool HasClaims =>
            Household?.HasClaims ?? BuyToLet!.HasClaims;

        public bool AutoRenew =>
            Household?.AutoRenew ?? BuyToLet!.AutoRenew;
    }

    [Test]
    public void RenewAHouseholdPolicy()
    {
        var arranged = ArrangeStoredHouseholdPolicy();
        var policyRenewer = CreatePolicyRenewer(arranged);

        var renewalPaymentReference = Guid.NewGuid();
        var request = CreateHouseholdRenewRequest(arranged, renewalPaymentReference, amount: 77m);

        var result = policyRenewer.RenewHouseholdPolicy(request);

        AssertSuccessfulRenewal(
            result: result,
            expectedUniqueReference: arranged.UniqueReference,
            oldPeriod: arranged.Period,
            expectedAmount: 77m,
            expectedRenewalPaymentReference: renewalPaymentReference,
            expectedPaymentType: PaymentType);
    }

    // Tests Application level concerns outside of the domain, that being DTO input shape specifically
    [Test]
    public void FailToRenewAHouseholdPolicyWithMultiplePayments()
    {
        var arranged = ArrangeStoredHouseholdPolicy();
        var policyRenewer = CreatePolicyRenewer(arranged);

        var request = CreateHouseholdRenewRequestWithMultiplePayments(arranged);

        var result = policyRenewer.RenewHouseholdPolicy(request);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error!.Code, Is.EqualTo("policy.renewal.payment.tooMany"));
            Assert.That(result.Error.Description, Is.Not.Null.Or.Empty);
        }
    }

    [Test]
    public void RenewABuyToLetPolicy()
    {
        var arranged = ArrangeStoredBuyToLetPolicy();
        var policyRenewer = CreatePolicyRenewer(arranged);

        var renewalPaymentReference = Guid.NewGuid();
        var request = CreateBuyToLetRenewRequest(arranged, renewalPaymentReference, amount: 77m);

        var result = policyRenewer.RenewBuyToLetPolicy(request);

        AssertSuccessfulRenewal(
            result: result,
            expectedUniqueReference: arranged.UniqueReference,
            oldPeriod: arranged.Period,
            expectedAmount: 77m,
            expectedRenewalPaymentReference: renewalPaymentReference,
            expectedPaymentType: PaymentType);
    }

    // Tests Application level concerns outside of the domain, that being DTO input shape specifically
    [Test]
    public void FailToRenewABuyToLetPolicyWithMultiplePayments()
    {
        var arranged = ArrangeStoredBuyToLetPolicy();
        var policyRenewer = CreatePolicyRenewer(arranged);

        var request = CreateBuyToLetRenewRequestWithMultiplePayments(arranged);

        var result = policyRenewer.RenewBuyToLetPolicy(request);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error!.Code, Is.EqualTo("policy.renewal.payment.tooMany"));
            Assert.That(result.Error.Description, Is.Not.Null.Or.Empty);
        }
    }

    // -------------------------
    // Arrange helpers
    // -------------------------

    private static ArrangedPolicy ArrangeStoredHouseholdPolicy()
    {
        var store = new PolicyStore();
        var arranged = ArrangeCommon(store);

        var initialPayment = PolicyPayment.Create(
            paymentReference: Guid.NewGuid(),
            paymentType: PaymentType,
            amount: 50m).Value;

        var sold = HouseholdPolicy.Sell(
            period: arranged.Period,
            amount: Money.Create(50m).Value,
            hasClaims: false,
            autoRenew: true,
            policyHolders: arranged.Holders,
            property: arranged.Property,
            payments: [initialPayment]);

        Assume.That(sold.IsSuccess, Is.True);
        Assume.That(store.StoreHouseholdPolicy(sold.Value).IsSuccess, Is.True);

        return new ArrangedPolicy(
            Store: store,
            Period: arranged.Period,
            Holder: arranged.Holder,
            Property: arranged.Property,
            Household: sold.Value,
            BuyToLet: null);
    }

    private static ArrangedPolicy ArrangeStoredBuyToLetPolicy()
    {
        var store = new PolicyStore();
        var arranged = ArrangeCommon(store);

        var initialPayment = PolicyPayment.Create(
            paymentReference: Guid.NewGuid(),
            paymentType: PaymentType,
            amount: 50m).Value;

        var sold = BuyToLetPolicy.Sell(
            period: arranged.Period,
            amount: Money.Create(50m).Value,
            hasClaims: false,
            autoRenew: true,
            policyHolders: arranged.Holders,
            property: arranged.Property,
            payments: [initialPayment]);

        Assume.That(sold.IsSuccess, Is.True);
        Assume.That(store.StoreBuyToLetPolicy(sold.Value).IsSuccess, Is.True);

        return new ArrangedPolicy(
            Store: store,
            Period: arranged.Period,
            Holder: arranged.Holder,
            Property: arranged.Property,
            Household: null,
            BuyToLet: sold.Value);
    }

    private sealed record CommonArrange(
        PolicyPeriod Period,
        PolicyHolder Holder,
        PolicyHolders Holders,
        InsuredProperty Property);

    private static CommonArrange ArrangeCommon(PolicyStore _)
    {
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

        return new CommonArrange(period, holder, holders, property);
    }

    private static PolicyRenewer CreatePolicyRenewer(ArrangedPolicy arranged)
    {
        var clock = new TestClock(today: arranged.ClockToday);
        return new PolicyRenewer(arranged.Store, clock);
    }

    // -------------------------
    // DTO helpers
    // -------------------------

    private static HouseholdPolicyDto CreateHouseholdRenewRequest(
        ArrangedPolicy arranged,
        Guid renewalPaymentReference,
        decimal amount)
    {
        return new HouseholdPolicyDto
        {
            UniqueReference = arranged.UniqueReference,
            StartDate = arranged.Period.StartDate,
            EndDate = arranged.Period.EndDate,
            Amount = amount,
            HasClaims = arranged.HasClaims,
            AutoRenew = arranged.AutoRenew,
            PolicyHolders =
            [
                new PolicyHolderDto
                {
                    FirstName = arranged.Holder.FirstName,
                    LastName = arranged.Holder.LastName,
                    DateOfBirth = arranged.Holder.DateOfBirth
                }
            ],
            Property = new PropertyDto
            {
                AddressLine1 = arranged.Property.AddressLine1,
                AddressLine2 = arranged.Property.AddressLine2,
                AddressLine3 = arranged.Property.AddressLine3,
                PostCode = arranged.Property.PostCode.Value
            },
            Payments =
            [
                new PaymentDto
                {
                    PaymentReference = renewalPaymentReference,
                    PaymentType = PaymentType,
                    Amount = amount
                }
            ]
        };
    }

    private static HouseholdPolicyDto CreateHouseholdRenewRequestWithMultiplePayments(ArrangedPolicy arranged)
    {
        var primaryPaymentReference = Guid.NewGuid();

        return CreateHouseholdRenewRequest(arranged, primaryPaymentReference, amount: 77m) with
        {
            Payments =
            [
                new PaymentDto
                {
                    PaymentReference = primaryPaymentReference,
                    PaymentType = PaymentType,
                    Amount = 77m
                },
                new PaymentDto
                {
                    PaymentReference = Guid.NewGuid(),
                    PaymentType = PaymentType,
                    Amount = 55m
                }
            ]
        };
    }

    private static BuyToLetPolicyDto CreateBuyToLetRenewRequest(
        ArrangedPolicy arranged,
        Guid renewalPaymentReference,
        decimal amount)
    {
        return new BuyToLetPolicyDto
        {
            UniqueReference = arranged.UniqueReference,
            StartDate = arranged.Period.StartDate,
            EndDate = arranged.Period.EndDate,
            Amount = amount,
            HasClaims = arranged.HasClaims,
            AutoRenew = arranged.AutoRenew,
            PolicyHolders =
            [
                new PolicyHolderDto
                {
                    FirstName = arranged.Holder.FirstName,
                    LastName = arranged.Holder.LastName,
                    DateOfBirth = arranged.Holder.DateOfBirth
                }
            ],
            Property = new PropertyDto
            {
                AddressLine1 = arranged.Property.AddressLine1,
                AddressLine2 = arranged.Property.AddressLine2,
                AddressLine3 = arranged.Property.AddressLine3,
                PostCode = arranged.Property.PostCode.Value
            },
            Payments =
            [
                new PaymentDto
                {
                    PaymentReference = renewalPaymentReference,
                    PaymentType = PaymentType,
                    Amount = amount
                }
            ]
        };
    }

    private static BuyToLetPolicyDto CreateBuyToLetRenewRequestWithMultiplePayments(ArrangedPolicy arranged)
    {
        var primaryPaymentReference = Guid.NewGuid();

        return CreateBuyToLetRenewRequest(arranged, primaryPaymentReference, amount: 77m) with
        {
            Payments =
            [
                new PaymentDto
                {
                    PaymentReference = primaryPaymentReference,
                    PaymentType = PaymentType,
                    Amount = 77m
                },
                new PaymentDto
                {
                    PaymentReference = Guid.NewGuid(),
                    PaymentType = PaymentType,
                    Amount = 55m
                }
            ]
        };
    }

    // -------------------------
    // Assert helpers
    // -------------------------

    private static void AssertSuccessfulRenewal<TPolicyDto>(
        Resulting<TPolicyDto> result,
        Guid expectedUniqueReference,
        PolicyPeriod oldPeriod,
        decimal expectedAmount,
        Guid expectedRenewalPaymentReference,
        string expectedPaymentType)
        where TPolicyDto : PolicyDto
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);

            var renewed = result.Value;

            Assert.That(renewed.UniqueReference, Is.EqualTo(expectedUniqueReference));

            // Period should roll forward by 1 year: new start == old end
            Assert.That(renewed.StartDate, Is.EqualTo(oldPeriod.EndDate));
            Assert.That(renewed.EndDate, Is.EqualTo(oldPeriod.EndDate.AddYears(1)));

            // Renewal updates amount
            Assert.That(renewed.Amount, Is.EqualTo(expectedAmount));

            // Renewal adds a payment (domain appends it)
            Assert.That(renewed.Payments, Has.Count.EqualTo(2));
            Assert.That(renewed.Payments.Last().PaymentReference, Is.EqualTo(expectedRenewalPaymentReference));
            Assert.That(renewed.Payments.Last().PaymentType, Is.EqualTo(expectedPaymentType));
            Assert.That(renewed.Payments.Last().Amount, Is.EqualTo(expectedAmount));
        }
    }
}