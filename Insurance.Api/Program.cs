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
                result.Value);
        
        return Results.BadRequest(result.Error);
    }).WithName("SellHouseholdPolicy");

app.MapPost("/policies/v1/buytolet",
    (BuyToLetPolicyDto policy, ISellBuyToLetPolicies policySeller) =>
    {
        var result = policySeller.Sell(policy);

        if (result.IsSuccess)
            return Results.Created($"/policies/v1/buytolet/{policy.UniqueReference}",
                result.Value);
        
        return Results.BadRequest(result.Error);
    }).WithName("SellBuyToLetPolicy");
    
app.MapGet("/policies/v1/household/{uniqueReference:guid}",
    (Guid uniqueReference, IRetrieveHouseholdPolicies policyRetriever) =>
    {
        var result = policyRetriever.Retrieve(uniqueReference);

        if (result.IsSuccess)
            return Results.Ok(result.Value);

        return Results.NotFound(result.Error);
    }).WithName("RetrieveHouseholdPolicy");

app.MapGet("/policies/v1/buytolet/{uniqueReference:guid}",
    (Guid uniqueReference, IRetrieveBuyToLetPolicies policyRetriever) =>
    {
        var result = policyRetriever.Retrieve(uniqueReference);

        if (result.IsSuccess)
            return Results.Ok(result.Value);

        return Results.NotFound(result.Error);
    }).WithName("RetrieveBuyToLetPolicy");

app.MapDelete("/policies/v1/buytolet/{uniqueReference:guid}",
    (Guid uniqueReference, ICancelBuyToLetPolicies policyCanceller) =>
    {
        var result = policyCanceller.Cancel(uniqueReference);

        if (result.IsSuccess)
            return Results.NoContent();

        return Results.BadRequest(result.Error);
    }).WithName("CancelBuyToLetPolicy");

app.UseHttpsRedirection();

app.Run();