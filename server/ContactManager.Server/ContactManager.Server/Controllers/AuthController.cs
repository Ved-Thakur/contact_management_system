using ContactManager.Server.Dtos;
using ContactManager.Server.Services;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace ContactManager.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly TokenService _tokenService;

        public AuthController(AuthService authService, TokenService tokenService)
        {
            _authService = authService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            RegisterDto dto,
            [FromServices] IValidator<RegisterDto> validator)
        {
            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(new
                {
                    Errors = validation.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    )
                });

            var user = await _authService.RegisterUser(dto);
            return user != null
                ? Ok(new { user.Id, user.Email })
                : BadRequest("Email already exists");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginDto dto,
            [FromServices] IValidator<LoginDto> validator)
        {
            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(new
                {
                    Errors = validation.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    )
                });

            var user = await _authService.ValidateLogin(dto);
            return user != null
                ? Ok(new { token = _tokenService.CreateToken(user) })
                : Unauthorized("Invalid credentials");
        }
    }
}