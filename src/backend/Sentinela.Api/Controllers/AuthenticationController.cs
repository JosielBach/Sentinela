using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sentinela.Application.UseCases.Login.WithEmailAndPassword;
using Sentinela.Communication.Requests;
using Sentinela.Communication.Responses;

namespace Sentinela.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(ResponseRegisterUserJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] RequestLoginJson request, [FromServices] ILoginWithEmailAndPasswordUseCase useCase)
    {
        var response = await useCase.Execute(request);
        return Ok(response);
    }
}
