using Insurance.Domain;

namespace Insurance.Application.Interfaces;

public interface IStorePolicies
{
    Result StoreHouseholdPolicy(HouseholdPolicy householdPolicy);
    Result StoreBuyToLetPolicy(BuyToLetPolicy buyToLetPolicy);
    Resulting<BuyToLetPolicy> FetchBuyToLetPolicy(Guid uniqueReference);
    Resulting<HouseholdPolicy> FetchHouseholdPolicy(Guid uniqueReference);
}