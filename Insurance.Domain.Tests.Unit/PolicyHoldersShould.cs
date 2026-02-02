namespace Insurance.Domain.Tests.Unit;

[TestFixture]
public class PolicyHoldersShould
{
    [Test]
    public void HoldThisManyPolicyHolders([Range(1, 3)] int numberOfHolders)
    {
        var policyHolders = Enumerable.Range(1, numberOfHolders).Select<int, PolicyHolder>(idx =>
            PolicyHolder.Create($"{idx}", "lastName",
                DateOnly.FromDateTime(DateTime.UtcNow.Date.AddYears(-30))).Value).ToArray();

        var result = PolicyHolders.Create(policyHolders);
        
        Assert.That(result.IsSuccess, Is.True);
    }
    
    [Test]
    public void FailWithZeroPolicyHolders()
    {
        var result = PolicyHolders.Create([]);
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error!.Code, Is.EqualTo("policy.holders.tooFew"));
            Assert.That(result.Error.Description, Is.Not.Null.Or.Empty);
        }
    }
    
    [Test]
    public void FailWithTooManyPolicyHolders()
    {
        var policyHolders = Enumerable.Range(1, 4).Select<int, PolicyHolder>(idx =>
            PolicyHolder.Create($"{idx}", "lastName",
                DateOnly.FromDateTime(DateTime.UtcNow.Date.AddYears(-30))).Value).ToArray();
        
        var result = PolicyHolders.Create(policyHolders);
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error!.Code, Is.EqualTo("policy.holders.tooMany"));
            Assert.That(result.Error.Description, Is.Not.Null.Or.Empty);
        }
    }
}