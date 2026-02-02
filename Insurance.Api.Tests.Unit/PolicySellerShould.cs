using Insurance.Api.Dtos.v1;

namespace Insurance.Api.Tests.Unit;

[TestFixture]
public class PolicySellerShould
{
    [Test]
    public void SellANewBuyToLetPolicy()
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
                new PolicyHolderDto
                {
                    DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddYears(-30)),
                    FirstName = "Test",
                    LastName = "User"
                }
            ]
        };
        var policySeller = new PolicySeller();
        
        var policyResult = policySeller.SellBuyToLetPolicy(newPolicyRequest);
        
        Assert.That(policyResult.IsSuccess, Is.True);
    }
    
    /// <summary>
    /// Makes sure that domain errors are bubbling up, the real tests exist elsewhere, and I won't re-test them here
    /// </summary>
    [Test]
    public void NotSellABuyToLetPolicyWithAYoungPolicyHolder()
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
                new PolicyHolderDto
                {
                    DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddYears(-15)),
                    FirstName = "Test",
                    LastName = "User"
                }
            ]
        };
        var policySeller = new PolicySeller();
        
        var policyResult = policySeller.SellBuyToLetPolicy(newPolicyRequest);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(policyResult.IsSuccess, Is.False);
            Assert.That(policyResult.Error!.Code, Is.EqualTo("policy.holders.age.tooYoung"));
            Assert.That(policyResult.Error.Description, Is.Not.Null.Or.Empty);
        }
    }
}