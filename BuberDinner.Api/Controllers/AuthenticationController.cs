using BuberDinner.Api.Filters;
using BuberDinner.Application.Common.Errors;
using BuberDinner.Application.Services.Authentication;
using BuberDinner.Contracts.Authentication;
using Microsoft.AspNetCore.Mvc;
using OneOf;

namespace BuberDinner.Api.Controllers;

[ApiController]
[Route("auth")]
[ErrorHandlingFilter]
public class AuthenticationController : ControllerBase
{
  private readonly IAuthenticationService _authenticationService;

  public AuthenticationController(IAuthenticationService authenticationService) 
  {
    _authenticationService = authenticationService;
  }

  [HttpPost("register")]
  public IActionResult Register(RegisterRequest request) 
  {
    OneOf<AuthenticationResult, IError> registerResult = _authenticationService.Register (
      request.FirstName, 
      request.LastName, 
      request.Email, 
      request.Password
    );

    return registerResult.Match(
      authResult => Ok(MapAuthResult(authResult)),
      error => Problem(statusCode: (int)error.StatusCode, title: error.ErrorMessage)
    );
  }

  private AuthenticationResponse MapAuthResult(AuthenticationResult authResult)
  {
    return new AuthenticationResponse(
      authResult.User.Id,
      authResult.User.FirstName,
      authResult.User.LastName,
      authResult.User.Email,
      authResult.Token
    );
  }

  [HttpPost("login")]
  public IActionResult Login(LoginRequest request) 
  {
    var authResult = _authenticationService.Login (
      request.Email,
      request.Password
    );
    var response = new AuthenticationResponse (
      authResult.User.Id,
      authResult.User.FirstName,
      authResult.User.LastName,
      authResult.User.Email,
      authResult.Token
    );
    return Ok(response);
  }
}