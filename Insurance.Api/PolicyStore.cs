using Insurance.Api.Interfaces;
using Insurance.Domain;

namespace Insurance.Api;

public class PolicyStore : IStorePolicies
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
        catch (Exception e)
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
        catch (Exception e)
        {
            // Log the exception here for diagnosis and return a failure
            return Result.Failure(Error.Failure("policy.store.failed", "Failed to store policy."));
        }
        
        return Result.Success();
    }

    public Resulting<BuyToLetPolicy> FetchBuyToLetPolicy(Guid uniqueReference)
    {
        throw new NotImplementedException();
    }

    public Resulting<HouseholdPolicy> FetchHouseholdPolicy(Guid uniqueReference)
    {
        throw new NotImplementedException();
    }
}