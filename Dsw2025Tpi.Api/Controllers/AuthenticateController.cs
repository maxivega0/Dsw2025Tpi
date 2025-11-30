using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticateController: ControllerBase
    {
        private readonly IAuthenticateManagementService _authenticateService;

        public AuthenticateController(IAuthenticateManagementService service)
        {
            _authenticateService = service;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel request)
        {
            var data = await _authenticateService.Login(request);

            var userNormalized = new { username = data.User.UserName, role = data.Role.Name };

            return Ok( new {token = data.Token, user = userNormalized});


        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel request)
        {
            var data = await _authenticateService.Register(request);

            var userNormalized = new { username = data.User.UserName, role = data.Role.Name };

            return Ok(new { token = data.Token, user = userNormalized });

        }
    }
}
