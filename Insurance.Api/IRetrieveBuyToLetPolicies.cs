using Insurance.Api.Dtos.v1;

namespace Insurance.Api;

public interface IRetrieveBuyToLetPolicies
{
    Resulting<BuyToLetPolicyDto> Retrieve(Guid uniqueReference);
}