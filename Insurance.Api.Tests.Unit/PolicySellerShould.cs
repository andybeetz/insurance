using FakeItEasy;
using Insurance.Api.Dtos.v1;
using Insurance.Api.Interfaces;
using Insurance.Domain;

namespace Insurance.Api.Tests.Unit;

[TestFixture]
public class PolicySellerShould
{
    private PolicySeller _policySeller;
    private IStorePolicies _policyStore;

    [SetUp]
    public void Setup()
    {
        _policyStore = A.Fake<IStorePolicies>();
        _policySeller = new PolicySeller(_policyStore);
    }

    [Test]
    public void SellANewBuyToLetPolicy()
    {
        var newPolicyRequest = CreateNewBuyToLetPolicyRequest();

        var policyResult = _policySeller.SellBuyToLetPolicy(newPolicyRequest);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(policyResult.IsSuccess, Is.True);
            Assert.That(policyResult.Value.UniqueReference, Is.Not.EqualTo(Guid.Empty));
        }
    }

    /// <summary>
    /// Makes sure that domain errors are bubbling up, the real tests exist elsewhere, and I won't re-test them here
    /// </summary>
    [Test]
    public void NotSellABuyToLetPolicyWithAYoungPolicyHolder()
    {
        var newPolicyRequest = CreateNewBuyToLetPolicyRequest(new PolicyHolderDto
        {
            DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddYears(-15)),
            FirstName = "Test",
            LastName = "User"
        });

        var policyResult = _policySeller.SellBuyToLetPolicy(newPolicyRequest);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(policyResult.IsSuccess, Is.False);
            Assert.That(policyResult.Error!.Code, Is.EqualTo("policy.holders.age.tooYoung"));
            Assert.That(policyResult.Error.Description, Is.Not.Null.Or.Empty);
        }
    }

    [Test]
    public void SellANewHouseholdPolicy()
    {
        var newPolicyRequest = CreateNewHouseholdPolicyRequest();

        var policyResult = _policySeller.SellHouseholdPolicy(newPolicyRequest);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(policyResult.IsSuccess, Is.True);
            Assert.That(policyResult.Value.UniqueReference, Is.Not.EqualTo(Guid.Empty));
        }
    }

    [Test]
    public void StoreASoldHouseholdPolicy()
    {
        var newPolicyRequest = CreateNewHouseholdPolicyRequest();

        var policyResult = _policySeller.SellHouseholdPolicy(newPolicyRequest);

        Assert.That(policyResult.IsSuccess, Is.True);
        A.CallTo(() =>
                _policyStore.StoreHouseholdPolicy(A<HouseholdPolicy>.That.Matches(policy =>
                    policy.UniqueReference == policyResult.Value.UniqueReference)))
            .MustHaveHappenedOnceExactly();
    }
    
    [Test]
    public void StoreASoldBuyToLetPolicy()
    {
        var newPolicyRequest = CreateNewBuyToLetPolicyRequest();

        var policyResult = _policySeller.SellBuyToLetPolicy(newPolicyRequest);

        Assert.That(policyResult.IsSuccess, Is.True);
        A.CallTo(() =>
                _policyStore.StoreBuyToLetPolicy(A<BuyToLetPolicy>.That.Matches(policy =>
                    policy.UniqueReference == policyResult.Value.UniqueReference)))
            .MustHaveHappenedOnceExactly();
    }
    
    private static HouseholdPolicyDto CreateNewHouseholdPolicyRequest()
    {
        var newPolicyRequest = new HouseholdPolicyDto
        {
            UniqueReference = Guid.Empty,
            Amount = 50.00m,
            AutoRenew = false,
            HasClaims = false,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddYears(1)),
            Payments = [new PaymentDto { PaymentReference = Guid.NewGuid(), PaymentType = "Card", Amount = 50.00m }],
            Property = new PropertyDto
                { AddressLine1 = "1 Test Street", AddressLine2 = null, AddressLine3 = null, PostCode = "ZZ1 1ZZ" },
            PolicyHolders =
            [
                new PolicyHolderDto
                {
                    DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddYears(-30)),
                    FirstName = "Test",
                    LastName = "User"
                }
            ]
        };
        return newPolicyRequest;
    }
    
    private static BuyToLetPolicyDto CreateNewBuyToLetPolicyRequest(PolicyHolderDto? policyHolder = null)
    {
        var newPolicyRequest = new BuyToLetPolicyDto
        {
            UniqueReference = Guid.Empty,
            Amount = 50.00m,
            AutoRenew = false,
            HasClaims = false,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddYears(1)),
            Payments = [new PaymentDto { PaymentReference = Guid.NewGuid(), PaymentType = "Card", Amount = 50.00m }],
            Property = new PropertyDto
                { AddressLine1 = "1 Test Street", AddressLine2 = null, AddressLine3 = null, PostCode = "ZZ1 1ZZ" },
            PolicyHolders =
            [
                policyHolder ?? new PolicyHolderDto
                {
                    DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddYears(-30)),
                    FirstName = "Test",
                    LastName = "User"
                }
            ]
        };
        return newPolicyRequest;
    }
}