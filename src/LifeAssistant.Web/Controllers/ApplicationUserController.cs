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

    /// <summary>
    /// Get the list of life assistants
    /// </summary>
    /// <returns>The profile of all validated life assistants</returns>
    [HttpGet("assistants")]
    public async Task<ActionResult<List<GetUserResponse>>> GetLifeAssistants()
    {
        return Ok(await usersApplication.GetLifeAssistants());
    }
}