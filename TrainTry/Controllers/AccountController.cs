using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using TrainTry.Configuration;
using TrainTry.Models;
using Microsoft.EntityFrameworkCore;
using TrainTry.Interfaces;


namespace TrainTry.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(string login, string password)
        {
            var result = await _accountService.Register(login, password);
            if (result == "Пользователь с таким логином уже существует.")
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("login")]
        public IActionResult Login(string login, string password)
        {
            var token = _accountService.Login(login, password);
            if (token == "Неверные данные.")
                return Unauthorized(token);
            return Ok(new { Token = token });
        }

        [HttpPost("setRole")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SetRole(string login, string role)
        {
            var result = await _accountService.SetRole(login, role);
            if (result == "Пользователь не найден.")
                return NotFound(result);
            return Ok(result);
        }

        [HttpGet("GetUsers", Name = "GetUsers")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            var users = await _accountService.GetUsers();
            return Ok(users);
        }

        [HttpDelete("DeleteUser", Name = "DeleteUser")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _accountService.DeleteUser(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Пользователь не найден.");
            }
        }
    }
}


