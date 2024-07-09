using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (_context.Users.Any(u => u.Login == user.Login))
            {
                return BadRequest("User already exists.");
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            var existingUser = _context.Users.SingleOrDefault(u => u.Login == user.Login && u.Password == user.Password);
            if (existingUser == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            return Ok(new { Token = "fake-jwt-token" }); // Здесь можно вернуть JWT токен при необходимости
        }

    }


}
