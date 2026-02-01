using Insurance.Api.Dtos.v1;

namespace Insurance.Api;

public interface IRetrieveHouseholdPolicies
{
    Resulting<HouseholdPolicyDto> Retrieve(Guid uniqueReference);
}
