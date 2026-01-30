using Insurance.Api.Domain;
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
        var result = policySeller.Sell(policy.ToDomain());

        if (result.IsSuccess)
            return Results.Created($"/policies/v1/household/{policy.UniqueReference}",
                policy with { UniqueReference = result.Value.UniqueReference });
        
        return Results.BadRequest(result.Error);
    }).WithName("SellHouseholdPolicy");

app.UseHttpsRedirection();

app.Run();