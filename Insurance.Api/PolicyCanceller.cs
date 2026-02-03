using Insurance.Api.Interfaces;
using Insurance.Domain;

namespace Insurance.Api;

public class PolicyCanceller : ICancelPolicies
{
    public Result CancelHouseholdPolicy(Guid uniqueReference)
    {
        throw new NotImplementedException();
    }

    public Result CancelBuyToLetPolicy(Guid uniqueReference)
    {
        throw new NotImplementedException();
    }
}