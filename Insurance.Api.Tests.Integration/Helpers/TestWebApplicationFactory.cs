using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Insurance.Api.Tests.Integration.Helpers;

public class TestWebApplicationFactory<TProgram>(
    ISellHouseholdPolicies householdPolicySeller,
    ISellBuyToLetPolicies buyToLetPolicySeller,
    IRetrieveHouseholdPolicies householdPolicyRetriever,
    IRetrieveBuyToLetPolicies buyToLetPolicyRetriever)
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
        });

        return base.CreateHost(builder);
    }
}