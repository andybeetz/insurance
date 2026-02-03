using Insurance.Application.Interfaces;
using Insurance.Domain;

namespace Insurance.Application;

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