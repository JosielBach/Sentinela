using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentinela.Application.UseCases.Asset.Register;
using Sentinela.Communication.Requests;
using Sentinela.Communication.Responses;

namespace Sentinela.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AssetsController : ControllerBase
{
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ResponseRegisterAssetJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RegisterAsset([FromBody] RequestRegisterAssetJson request, 
        [FromServices] IRegisterAssetUseCase useCase)
    {
        var result = await useCase.Execute(request);
        return Created(string.Empty, result);
    }
}
