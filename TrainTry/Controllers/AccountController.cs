using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace TrainTry.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class AccountController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<AccountController> _logger;



        public AccountController(ApplicationContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
            _logger.LogDebug(1, "NLog внедрен в AccountController");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(string login, string password)
        {
            _logger.LogInformation("Попытка регистрации с логином: {Login}", login);

            User user = new User { Login = login, Password = password };

            if (_context.Users.Any(u => u.Login == login))
            {
                _logger.LogWarning("Пользователь с логином {Login} уже существует", login);
                return BadRequest("Пользователь с таким логином уже существует.");
            }

            _logger.LogInformation("Пользователь {Login} загеристрирован успешно", login);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Регистрация пройдена успешно.");
        }

        [HttpPost("login")]
        public IActionResult Login(string login, string password)
        {

            _logger.LogInformation("Попытка входа с логином: {Login}", login);

            var existingUser = _context.Users.SingleOrDefault(u => u.Login == login && u.Password == password);
            if (existingUser == null)
            {
                _logger.LogWarning("Неудачная попытка входа: {Login}", login);
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

            _logger.LogInformation("Пользователь {Login} вошел успешно", login);
            return Ok(new { Token = token });
        }

        [HttpPost("setRole")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SetRole(string login, string role)
        {

            _logger.LogInformation("Попытка установить роль для логина: {Login} роль: {Role}", login, role);

            var user = _context.Users.SingleOrDefault(u => u.Login == login);
            if (user == null)
            {
                _logger.LogWarning("Пользователь с таким логином не найден: {Login}", login);
                return NotFound("Пользователь не найден.");
            }

            user.AccessRole = role;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Роль {Role} установлена для пользователя: {Login}", role, login);
            return Ok("Роль выдана успешно.");
        }

    }


}
