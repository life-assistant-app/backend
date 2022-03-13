using LifeAssistant.Core.Application.Users;
using LifeAssistant.Core.Application.Users.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace LifeAssistant.Web.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UsersApplication application;

    public AuthController(UsersApplication application)
    {
        this.application = application;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
    {
        return Created("",await this.application.Register(request));
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] LoginRequest request)
    {
        return Ok(await this.application.Login(request));
    }
}