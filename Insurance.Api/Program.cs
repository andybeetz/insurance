using Insurance.Api;
using Insurance.Api.Dtos.v1;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/policies/v1/household",
    (HouseholdPolicyDto policy, ISellHouseholdPolicies policySeller) =>
    {
        var result = policySeller.Sell(policy);

        if (result.IsSuccess)
            return Results.Created($"/policies/v1/household/{policy.UniqueReference}",
                policy with { UniqueReference = result.Value.UniqueReference });
        
        return Results.BadRequest(result.Error);
    }).WithName("SellHouseholdPolicy");

app.MapPost("/policies/v1/buytolet",
    (BuyToLetPolicyDto policy, ISellBuyToLetPolicies policySeller) =>
    {
        var result = policySeller.Sell(policy);

        if (result.IsSuccess)
            return Results.Created($"/policies/v1/buytolet/{policy.UniqueReference}",
                policy with { UniqueReference = result.Value.UniqueReference });
        
        return Results.BadRequest(result.Error);
    }).WithName("SellBuyToLetPolicy");

app.UseHttpsRedirection();

app.Run();