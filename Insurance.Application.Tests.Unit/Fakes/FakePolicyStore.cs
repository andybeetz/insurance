using Insurance.Application.Interfaces;
using Insurance.Domain;

namespace Insurance.Application.Tests.Unit.Fakes;

public class FakePolicyStore : IStorePolicies
{
    private readonly List<BuyToLetPolicy> _buyToLetPolicies = [];
    private readonly List<HouseholdPolicy> _householdPolicies = [];
    
    public Result StoreHouseholdPolicy(HouseholdPolicy householdPolicy)
    {
        try
        {
            // This might be real infrastructure that can throw in future
            _householdPolicies.Add(householdPolicy);
        }
        catch (Exception)
        {
            // Log the exception here for diagnosis and return a failure
            return Result.Failure(Error.Failure("policy.store.failed", "Failed to store policy."));
        }
        
        return Result.Success();
    }

    public Result StoreBuyToLetPolicy(BuyToLetPolicy buyToLetPolicy)
    {
        try
        {
            // This might be real infrastructure that can throw in future
            _buyToLetPolicies.Add(buyToLetPolicy);
        }
        catch (Exception)
        {
            // Log the exception here for diagnosis and return a failure
            return Result.Failure(Error.Failure("policy.store.failed", "Failed to store policy."));
        }
        
        return Result.Success();
    }

    public Resulting<BuyToLetPolicy> FetchBuyToLetPolicy(Guid uniqueReference)
    {
        try
        {
            // This might be real infrastructure that can throw in future
            var policy = _buyToLetPolicies.Single(p => p.UniqueReference == uniqueReference);
            return Resulting<BuyToLetPolicy>.Success(policy);
        }
        catch (Exception)
        {
            // Log the exception here for diagnosis and return a failure
            return Resulting<BuyToLetPolicy>.Failure(Error.NotFound("policy.store.notfound", "Policy not found."));
        }
    }

    public Resulting<HouseholdPolicy> FetchHouseholdPolicy(Guid uniqueReference)
    {
        try
        {
            // This might be real infrastructure that can throw in future
            var policy = _householdPolicies.Single(p => p.UniqueReference == uniqueReference);
            return Resulting<HouseholdPolicy>.Success(policy);
        }
        catch (Exception)
        {
            // Log the exception here for diagnosis and return a failure
            return Resulting<HouseholdPolicy>.Failure(Error.NotFound("policy.store.notfound", "Policy not found."));
        }
    }
}