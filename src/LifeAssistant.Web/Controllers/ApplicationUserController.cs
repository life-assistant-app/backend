using LifeAssistant.Core.Application.Users;
using LifeAssistant.Core.Application.Users.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace LifeAssistant.Web.Controllers;

[ApiController]
[Route("/api")]
public class ApplicationUserController : ControllerBase
{
    private readonly UsersApplication usersApplication;

    public ApplicationUserController(UsersApplication usersApplication)
    {
        this.usersApplication = usersApplication;
    }

    [HttpGet("assistants")]
    public async Task<ActionResult<IList<GetUserResponse>>> GetLifeAssistants()
    {
        return Ok(await usersApplication.GetLifeAssistants());
    }
}