using ClubAdmin.Finances.Application.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Wolverine;

namespace ClubAdmin.Finances.Api.Functions;

public class FinanceFunctions(IMessageBus bus)
{
    [Function("ImportTransaction")]
    [OpenApiOperation("ImportTransaction", tags: ["Finances"])]
    [OpenApiRequestBody("application/json", typeof(ImportTransaction), Required = true)]
    [OpenApiResponseWithBody(System.Net.HttpStatusCode.Accepted, "application/json", typeof(object))]
    public async Task<IActionResult> ImportTransaction(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "transactions/import")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        var command = await req.ReadFromJsonAsync<ImportTransaction>(cancellationToken);
        if (command is null)
        {
            return new BadRequestObjectResult("Invalid request body.");
        }

        await bus.InvokeAsync(command, cancellationToken);
        return new AcceptedResult();
    }

    [Function("CategorizeTransaction")]
    [OpenApiOperation("CategorizeTransaction", tags: ["Finances"])]
    [OpenApiRequestBody("application/json", typeof(CategorizeTransaction), Required = true)]
    [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(object))]
    public async Task<IActionResult> CategorizeTransaction(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "transactions/{id}/categorize")] HttpRequest req,
        string id,
        CancellationToken cancellationToken)
    {
        var body = await req.ReadFromJsonAsync<CategorizeTransaction>(cancellationToken);
        if (body is null)
        {
            return new BadRequestObjectResult("Invalid request body.");
        }

        var command = body with { TransactionId = id };
        await bus.InvokeAsync(command, cancellationToken);
        return new OkResult();
    }

    [Function("DefineBudget")]
    [OpenApiOperation("DefineBudget", tags: ["Finances"])]
    [OpenApiRequestBody("application/json", typeof(DefineBudget), Required = true)]
    [OpenApiResponseWithBody(System.Net.HttpStatusCode.Created, "application/json", typeof(object))]
    public async Task<IActionResult> DefineBudget(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "budget")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        var command = await req.ReadFromJsonAsync<DefineBudget>(cancellationToken);
        if (command is null)
        {
            return new BadRequestObjectResult("Invalid request body.");
        }

        await bus.InvokeAsync(command, cancellationToken);
        return new CreatedResult("/api/budget/report", null);
    }
}
