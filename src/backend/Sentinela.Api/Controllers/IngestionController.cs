using Microsoft.AspNetCore.Mvc;
using Sentinela.Api.Attributes;
using Sentinela.Api.Filters;
using Sentinela.Application.UseCases.Sample.Ingest;
using Sentinela.Communication.Requests;
using Sentinela.Communication.Responses;

namespace Sentinela.Api.Controllers;

[Route("api/v1/ingest")]
[ApiController]
public class IngestionController : ControllerBase
{
    [HttpPost]
    [AgentKey]
    [TypeFilter(typeof(IdempotencyKeyFilter))]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Ingest([FromBody] RequestSampleBatchJson request, 
        [FromServices] IIngestSampleBatchUseCase useCase, 
        [FromHeader (Name = "Idempotency-Key")] string idempotencyKey)
    {
        var assetId = (Guid)HttpContext.Items["AssetId"]!;

        await useCase.Execute(request, assetId, idempotencyKey);

        return Accepted();
    }
}
