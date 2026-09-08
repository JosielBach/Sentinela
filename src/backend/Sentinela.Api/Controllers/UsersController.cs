using Microsoft.AspNetCore.Mvc;
using Sentinela.Application.UseCases.User.Register;
using Sentinela.Communication.Requests;
using Sentinela.Communication.Responses;

namespace Sentinela.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(ResponseRegisterUserJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterUsers([FromBody] RequestRegisterUserAccountJson Request,
        [FromServices] IRegisterUserAccountUseCase useCase)
    {
       var result = await useCase.Execute(Request);

        return Created(string.Empty, result);
    } 

}
