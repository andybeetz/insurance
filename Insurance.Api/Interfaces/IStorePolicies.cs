using Insurance.Domain;

namespace Insurance.Api.Interfaces;

public interface IStorePolicies
{
    Result StoreHouseholdPolicy(HouseholdPolicy householdPolicy);
}