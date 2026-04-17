using ClubAdmin.Members.Application;
using ClubAdmin.Members.Application.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

namespace ClubAdmin.Members.Api.Functions;

public class MemberFunctions(MemberCommandService commandService)
{
    [Function("RegisterMember")]
    [OpenApiOperation("RegisterMember", tags: ["Members"])]
    [OpenApiRequestBody("application/json", typeof(RegisterMember), Required = true)]
    [OpenApiResponseWithBody(System.Net.HttpStatusCode.Created, "application/json", typeof(object))]
    public async Task<IActionResult> RegisterMember(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "members")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        var command = await req.ReadFromJsonAsync<RegisterMember>(cancellationToken);
        if (command is null)
        {
            return new BadRequestObjectResult("Invalid request body.");
        }

        var result = await commandService.Handle(command, cancellationToken);
        if (result.Success)
            return new CreatedAtRouteResult("GetMember", new { id = command.MemberId }, null);

        result.TryGetError(out var error);
        return new BadRequestObjectResult(error?.ErrorMessage ?? "An error occurred.");
    }

    [Function("UpdateMemberProfile")]
    [OpenApiOperation("UpdateMemberProfile", tags: ["Members"])]
    [OpenApiRequestBody("application/json", typeof(UpdateMemberProfile), Required = true)]
    [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(object))]
    public async Task<IActionResult> UpdateMemberProfile(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "members/{id}")] HttpRequest req,
        string id,
        CancellationToken cancellationToken)
    {
        var body = await req.ReadFromJsonAsync<UpdateMemberProfile>(cancellationToken);
        if (body is null)
        {
            return new BadRequestObjectResult("Invalid request body.");
        }

        var command = body with { MemberId = id };
        var result = await commandService.Handle(command, cancellationToken);
        if (result.Success)
            return new OkResult();

        result.TryGetError(out var error1);
        return new BadRequestObjectResult(error1?.ErrorMessage ?? "An error occurred.");
    }

    [Function("TerminateMembership")]
    [OpenApiOperation("TerminateMembership", tags: ["Members"])]
    [OpenApiResponseWithBody(System.Net.HttpStatusCode.NoContent, "application/json", typeof(object))]
    public async Task<IActionResult> TerminateMembership(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "members/{id}")] HttpRequest req,
        string id,
        CancellationToken cancellationToken)
    {
        var body = await req.ReadFromJsonAsync<TerminateMembership>(cancellationToken);
        var command = new TerminateMembership(id, body?.Reason ?? "No reason provided.");
        var result = await commandService.Handle(command, cancellationToken);
        if (result.Success)
            return new NoContentResult();

        result.TryGetError(out var error2);
        return new BadRequestObjectResult(error2?.ErrorMessage ?? "An error occurred.");
    }

    [Function("AssignContributionCategory")]
    [OpenApiOperation("AssignContributionCategory", tags: ["Members"])]
    [OpenApiRequestBody("application/json", typeof(AssignContributionCategory), Required = true)]
    [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(object))]
    public async Task<IActionResult> AssignContributionCategory(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "members/{id}/contribution")] HttpRequest req,
        string id,
        CancellationToken cancellationToken)
    {
        var body = await req.ReadFromJsonAsync<AssignContributionCategory>(cancellationToken);
        if (body is null)
        {
            return new BadRequestObjectResult("Invalid request body.");
        }

        var command = body with { MemberId = id };
        var result = await commandService.Handle(command, cancellationToken);
        if (result.Success)
            return new OkResult();

        result.TryGetError(out var error3);
        return new BadRequestObjectResult(error3?.ErrorMessage ?? "An error occurred.");
    }
}
