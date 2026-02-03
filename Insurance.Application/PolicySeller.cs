using Insurance.Application.Dtos.v1;
using Insurance.Application.Interfaces;
using Insurance.Domain;

namespace Insurance.Application;

public class PolicySeller(IStorePolicies policyStore) : ISellPolicies
{
    public Resulting<HouseholdPolicyDto> SellHouseholdPolicy(HouseholdPolicyDto policy)
    {
        var domainInput = MapSellInput(policy);
        if (!domainInput.IsSuccess)
            return domainInput.Error!;

        var soldResult = HouseholdPolicy.Sell(
            period: domainInput.Value.Period,
            amount: domainInput.Value.Amount,
            hasClaims: policy.HasClaims,
            autoRenew: policy.AutoRenew,
            policyHolders: domainInput.Value.PolicyHolders,
            property: domainInput.Value.Property,
            payments: domainInput.Value.Payments);

        if (!soldResult.IsSuccess)
            return soldResult.Error!;
        
        var storeResult = policyStore.StoreHouseholdPolicy(soldResult.Value);
        if (!storeResult.IsSuccess)
            return storeResult.Error ?? Error.Failure("policy.store.failed", "Failed to store policy.");

        return Resulting<HouseholdPolicyDto>.Success(policy with
        {
            UniqueReference = soldResult.Value.UniqueReference
        });
    }

    public Resulting<BuyToLetPolicyDto> SellBuyToLetPolicy(BuyToLetPolicyDto policy)
    {
        var domainInput = MapSellInput(policy);
        if (!domainInput.IsSuccess)
            return domainInput.Error!;

        var soldResult = BuyToLetPolicy.Sell(
            period: domainInput.Value.Period,
            amount: domainInput.Value.Amount,
            hasClaims: policy.HasClaims,
            autoRenew: policy.AutoRenew,
            policyHolders: domainInput.Value.PolicyHolders,
            property: domainInput.Value.Property,
            payments: domainInput.Value.Payments);

        if (!soldResult.IsSuccess)
            return soldResult.Error!;
        
        var storeResult = policyStore.StoreBuyToLetPolicy(soldResult.Value);
        if (!storeResult.IsSuccess)
            return storeResult.Error ?? Error.Failure("policy.store.failed", "Failed to store policy.");

        return Resulting<BuyToLetPolicyDto>.Success(policy with
        {
            UniqueReference = soldResult.Value.UniqueReference
        });
    }

    private static Resulting<SellInput> MapSellInput(PolicyDto policy)
    {
        var periodResult = PolicyPeriod.Create(policy.StartDate, policy.EndDate);
        if (!periodResult.IsSuccess)
            return periodResult.Error!;

        var moneyResult = Money.Create(policy.Amount);
        if (!moneyResult.IsSuccess)
            return moneyResult.Error!;

        var policyHolders = new List<PolicyHolder>(policy.PolicyHolders.Count);
        foreach (var h in policy.PolicyHolders)
        {
            var holderResult = PolicyHolder.Create(
                firstName: h.FirstName,
                lastName: h.LastName,
                dateOfBirth: h.DateOfBirth);

            if (!holderResult.IsSuccess)
                return holderResult.Error!;

            policyHolders.Add(holderResult.Value);
        }

        var holdersResult = PolicyHolders.Create(
            policyHolders: policyHolders,
            policyStartDate: periodResult.Value.StartDate);

        if (!holdersResult.IsSuccess)
            return holdersResult.Error!;

        var propertyResult = InsuredProperty.Create(
            addressLine1: policy.Property.AddressLine1,
            addressLine2: policy.Property.AddressLine2,
            addressLine3: policy.Property.AddressLine3,
            postCode: policy.Property.PostCode);

        if (!propertyResult.IsSuccess)
            return propertyResult.Error!;

        var payments = new List<PolicyPayment>(policy.Payments.Count);
        foreach (var p in policy.Payments)
        {
            var paymentResult = PolicyPayment.Create(
                paymentReference: p.PaymentReference,
                paymentType: p.PaymentType,
                amount: p.Amount);

            if (!paymentResult.IsSuccess)
                return paymentResult.Error!;

            payments.Add(paymentResult.Value);
        }

        return Resulting<SellInput>.Success(new SellInput(
            periodResult.Value,
            moneyResult.Value,
            holdersResult.Value,
            propertyResult.Value,
            payments));
    }

    private sealed record SellInput(
        PolicyPeriod Period,
        Money Amount,
        PolicyHolders PolicyHolders,
        InsuredProperty Property,
        IReadOnlyCollection<PolicyPayment> Payments);
}