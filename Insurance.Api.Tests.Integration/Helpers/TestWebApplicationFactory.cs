using Insurance.Api.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Insurance.Api.Tests.Integration.Helpers;

public class TestWebApplicationFactory<TProgram>(
    ISellPolicies policySeller,
    IRetrievePolicies policyRetriever,
    ICancelPolicies policyCanceller,
    IRenewPolicies policyRenewer)
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
            services.AddSingleton(policySeller);
            services.AddSingleton(policyRetriever);
            services.AddSingleton(policyCanceller);
            services.AddSingleton(policyRenewer);
        });

        return base.CreateHost(builder);
    }
}