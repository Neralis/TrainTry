using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TrainTry.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class AccountController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public AccountController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(string login, string password)
        {
            User user = new User { Login = login, Password = password };

            if (_context.Users.Any(u => u.Login == login))
            {
                return BadRequest("Пользователь с таким логином уже существует.");
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Регистрация пройдена успешно.");
        }

        [HttpPost("login")]
        public IActionResult Login(string login, string password)
        {
            var existingUser = _context.Users.SingleOrDefault(u => u.Login == login && u.Password == password);
            if (existingUser == null)
            {
                return Unauthorized("Неверные данные.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, existingUser.Login),
                new Claim(ClaimTypes.Role, existingUser.AccessRole) // Добавляем роль в токен
            };

            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claims,
                    notBefore: DateTime.UtcNow,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(60)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return Ok(new { Token = token });
        }

        [HttpPost("setRole")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SetRole(string login, string role)
        {
            var user = _context.Users.SingleOrDefault(u => u.Login == login);
            if (user == null)
            {
                return NotFound("Пользователь не найден.");
            }

            user.AccessRole = role;
            await _context.SaveChangesAsync();

            return Ok("Роль выдана успешно.");
        }

    }


}
