using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Insurance.Api.Tests.Integration.Helpers;

public class TestWebApplicationFactory<TProgram>(
    ISellHouseholdPolicies householdPolicySeller,
    ISellBuyToLetPolicies buyToLetPolicySeller,
    IRetrieveHouseholdPolicies householdPolicyRetriever,
    IRetrieveBuyToLetPolicies buyToLetPolicyRetriever,
    ICancelBuyToLetPolicies buyToLetPolicyCanceller,
    ICancelHouseholdPolicies householdPolicyCanceller)
    : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?> { { "EmailAddress", "test1@Contoso.com" } });
        });

        builder.ConfigureServices(services =>
        {
            services.AddSingleton(householdPolicySeller);
            services.AddSingleton(buyToLetPolicySeller);
            services.AddSingleton(householdPolicyRetriever);
            services.AddSingleton(buyToLetPolicyRetriever);
            services.AddSingleton(buyToLetPolicyCanceller);
            services.AddSingleton(householdPolicyCanceller);
        });

        return base.CreateHost(builder);
    }
}