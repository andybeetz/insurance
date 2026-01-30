using System.Net;
using System.Net.Http.Json;
using Insurance.Api.Dtos.v1;
using Insurance.Api.Tests.Integration.Helpers;

namespace Insurance.Api.Tests.Integration;

[TestFixture]
public class InsuranceApiShould
{
    private TestWebApplicationFactory<Program> _factory;
    private HttpClient _httpClient;
    
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _factory = new  TestWebApplicationFactory<Program>();
        _httpClient = _factory.CreateClient();
    }
    
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _factory.Dispose();
        _httpClient.Dispose();
    }

    [Test]
    public async Task SellAHouseholdPolicy()
    {
        var startDate = new DateOnly(2021, 05, 01);
        var endDate = startDate.AddDays(365);
        var dateOfBirth = new DateOnly(205, 05, 17);
        var policyReference = new Guid("020B67E9-8430-437B-A45A-F0BDE2061D37");
        var paymentReference = new Guid("120B67E9-8430-437B-A45A-F0BDE2061D38");
        var autoRenew = true;

        var expectedPolicy = new HouseholdPolicyDto
        {
            UniqueReference = policyReference,
            StartDate = startDate,
            EndDate = endDate,
            Amount = 120000m,
            HasClaims = false,
            AutoRenew = autoRenew,
            PolicyHolder = new PolicyHolderDto
            {
                FirstName = "Fred",
                LastName = "Flintstone",
                DateOfBirth = dateOfBirth
            },
            Property = new PropertyDto
            {
                AddressLine1 = "123 Main Street",
                PostCode = "NN12 3GG"
            },
            Payments =
            [
                new PaymentDto
                {
                    PaymentReference = paymentReference,
                    PaymentType = "OnlineCard",
                    Amount = 77.11m
                }
            ]
        };

        var newPolicyRequest = expectedPolicy;
        newPolicyRequest.UniqueReference = null;

        var response = await _httpClient.PostAsJsonAsync("/policies/v1/household", newPolicyRequest);

        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            var soldPolicy = await response.Content.ReadFromJsonAsync<HouseholdPolicyDto>();
            Assert.That(soldPolicy, Is.EqualTo(expectedPolicy));
        });
    }
}