using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentinela.Application.UseCases.User.ChangePassword;
using Sentinela.Application.UseCases.User.Profile;
using Sentinela.Application.UseCases.User.Register;
using Sentinela.Application.UseCases.User.Update;
using Sentinela.Communication.Requests;
using Sentinela.Communication.Responses;

namespace Sentinela.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseRegisterUserJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterUsers([FromBody] RequestRegisterUserAccountJson Request,
        [FromServices] IRegisterUserAccountUseCase useCase)
    {
       var result = await useCase.Execute(Request);

        return Created(string.Empty, result);
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ResponseUserProfileJson), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsersProfile([FromServices] IGetUserProfileUseCase useCase)
    {
        var result = await useCase.Execute();

        return Ok(result);
    }

    [HttpPut("profile")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProfile([FromServices] IUpdateUserUseCase useCase,[FromBody] RequestUpdateUserJson request)
    {
        await useCase.Execute(request);

        return NoContent();
    }

    [HttpPut("password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePassword([FromServices] IChangePasswordUseCase useCase,[FromBody] RequestChangePasswordJson request)
    {
        await useCase.Execute(request);

        return NoContent();
    }
}
