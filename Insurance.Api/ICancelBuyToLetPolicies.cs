namespace Insurance.Api;

public interface ICancelBuyToLetPolicies
{
    Result Cancel(Guid uniqueReference);
}