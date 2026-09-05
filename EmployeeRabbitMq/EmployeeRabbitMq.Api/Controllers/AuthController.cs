using EmployeeRabbitMq.Contracts.Models;
using EmployeeRabbitMq.Contracts.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeRabbitMq.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(JwtTokenService jwtTokenService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("token")]
        public IActionResult GenerateToken(
            [FromBody] TokenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                return BadRequest("Username is required.");
            }

            var token = jwtTokenService.GenerateToken(
                request.Username,
                request.Role);

            return Ok(new
            {
                accessToken = token
            });
        }
    }
}
