﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LifeAssistant.Core.Application.Users.Contracts;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Domain.Entities.ApplicationUser;
using LifeAssistant.Core.Persistence;
using Microsoft.IdentityModel.Tokens;

namespace LifeAssistant.Core.Application.Users;

public class UsersApplication
{
    private readonly IApplicationUserRepository applicationUserRepository;
    private readonly string jwtSecret;

    public UsersApplication(IApplicationUserRepository applicationUserRepository, string jwtSecret)
    {
        this.applicationUserRepository = applicationUserRepository;
        this.jwtSecret = jwtSecret;
    }

    public async Task<RegisterResponse> Register(RegisterRequest request)
    {
        var applicationUser = new ApplicationUser(
            request.Username,
            BCrypt.Net.BCrypt.HashPassword(request.Password),
            request.FirstName,
            request.LastName,
            Enum.Parse<ApplicationUserRole>(request.Role)
        );

        await this.applicationUserRepository.Insert(applicationUser);
        await this.applicationUserRepository.Save();

        return new RegisterResponse(
            applicationUser.Id,
            applicationUser.UserName,
            applicationUser.Role.ToString(),
            applicationUser.FirstName,
            applicationUser.LastName,
            applicationUser.Validated
        );
    }

    public async Task<LoginResponse> Login(LoginRequest request)
    {
        IApplicationUser applicationUser = await applicationUserRepository.FindByUsername(request.Username);
        EnsureUserFoundAndPasswordMatch(request.Password, applicationUser);

        if (!applicationUser.Validated)
        {
            throw new InvalidOperationException("User is not validated");
        }

        var tokenHandler = new JwtSecurityTokenHandler();

        SecurityTokenDescriptor tokenDescriptor = BuildTokenDescriptor(applicationUser);
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return new LoginResponse(tokenHandler.WriteToken(token));
    }

    public async Task<List<GetUserResponse>> GetLifeAssistants()
    {
        List<IApplicationUser> findValidatedByRole = await this.applicationUserRepository
            .FindValidatedByRole(ApplicationUserRole.LifeAssistant);
        return findValidatedByRole
            .Select(user => new GetUserResponse(user.Id, user.UserName, user.FirstName, user.LastName, user.Role))
            .ToList();
    }

    private SecurityTokenDescriptor BuildTokenDescriptor(IApplicationUser applicationUser)
    {
        return new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, applicationUser.Id.ToString()),
                new Claim(ClaimTypes.Name, applicationUser.UserName),
                new Claim(ClaimTypes.Role, applicationUser.Role.ToString()),
                new Claim(ClaimTypes.Surname, $"{applicationUser.FirstName} {applicationUser.LastName}"),
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
                    SecurityAlgorithms.HmacSha256)
        };
    }


    private static void EnsureUserFoundAndPasswordMatch(string password, IApplicationUser user)
    {
        ArgumentNullException.ThrowIfNull(user);
        if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            throw new ArgumentException("Invalid password");
        }
    }
}