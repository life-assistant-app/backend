using LifeAssistant.Core.Application.Users;
using LifeAssistant.Core.Application.Users.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LifeAssistant.Web.Controllers;

[AllowAnonymous]
[ApiController]
[Route("/api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UsersApplication application;

    public AuthController(UsersApplication application)
    {
        this.application = application;
    }

    /// <summary>
    /// Register a new user. The new user will be disable until activation by an agency employee
    /// </summary>
    /// <param name="request">
    ///     - Username : username of the new user
    ///     - Password : password of the new user
    ///     - Role : role of the new user
    /// </param>
    /// <returns>New user data</returns>
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
    {
        return Created("", await this.application.Register(request));
    }

    /// <summary>
    /// Login an user
    /// </summary>
    /// <param name="request">
    ///     - Username : username of the new user
    ///     - Password : password of the new user
    /// </param>
    /// <returns>JWT token for the user</returns>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        return Ok(await this.application.Login(request));
    }
}