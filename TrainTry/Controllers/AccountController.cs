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
using TrainTry.Services;


namespace TrainTry.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        HashFunction hashFunction;
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService; 
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserRegAndLogin user)
        {
            var result = await _accountService.Register(user.Login, user.Password);
            if (result == "Пользователь с таким логином уже существует.")
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody]UserRegAndLogin user)
        {
            var token = _accountService.Login(user.Login, user.Password);
            if (token == "Неверные данные.")
                return Unauthorized(token);
            return Ok(new { Token = token });
        }

        [HttpPost("setRole")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SetRole([FromBody]UserSetRole user)
        {
            var result = await _accountService.SetRole(user.Login, user.AccessRole);
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


