using Microsoft.AspNetCore.Mvc;
using Sentinela.Communication.Requests;

namespace Sentinela.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{

    [HttpPost]
    public IActionResult RegisterUsers([FromBody] RequestRegisterUserAccountJson Request)
    {

        return Ok();
    }

}
