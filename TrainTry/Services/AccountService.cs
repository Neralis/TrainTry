using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TrainTry.Configuration;
using TrainTry.Interfaces;
using TrainTry.Models;
using System.Text.RegularExpressions;

namespace TrainTry.Services
{
    public class AccountService : IAccountService
    {
        HashFunction hashFunction;
        private readonly ApplicationContext _context;
        private readonly ILogger<AccountService> _logger;

        public AccountService(ApplicationContext context, ILogger<AccountService> logger)
        {
            _context = context;
            _logger = logger;
        }

        private bool ContainsUnicode(string input)
        {
            // Проверка на наличие символов, не являющихся ASCII
            return input.Any(c => c > 127);
        }

        public async Task<string> Register(string login, string password)
        {
            _logger.LogInformation("Попытка регистрации с логином: {Login}", login);

            if (ContainsUnicode(login) || ContainsUnicode(password))
            {
                _logger.LogWarning("Регистрация с логином {Login} не удалась из-за недопустимых символов", login);
                return "Логин и пароль не могут содержать символы Юникода.";
            }

            if (_context.Users.Any(u => u.Login == login))
            {
                _logger.LogWarning("Пользователь с логином {Login} уже существует", login);
                return "Пользователь с таким логином уже существует.";
            }
            var (encryptedPassword,iv) = HashFunction.EncryptPassword(password);
            var user = new User 
            { 
                Login = login,
                Password = encryptedPassword,
                IV = iv
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Пользователь {Login} зарегистрирован успешно", login);
            return "Регистрация пройдена успешно.";
        }

        public string Login(string login, string password)
        {
            _logger.LogInformation("Попытка входа с логином: {Login}", login);

            if (ContainsUnicode(login) || ContainsUnicode(password))
            {
                _logger.LogWarning("Вход с логином {Login} не удался из-за недопустимых символов", login);
                return "Логин и пароль не могут содержать символы Юникода.";
            }

            var existingUser = _context.Users.SingleOrDefault(u => u.Login == login);
            if (existingUser == null || !HashFunction.VerifyPassword(password, existingUser.Password, existingUser.IV))
            {
                _logger.LogWarning("Неудачная попытка входа с логином: {Login}", login);
                return "Неверные данные.";
            }


            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, existingUser.Login),
            new Claim(ClaimTypes.Role, existingUser.AccessRole)
        };

            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(60)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            
            _logger.LogInformation("Пользователь {Login} вошел успешно", login);
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public async Task<string> SetRole(string login, string role)
        {
            _logger.LogInformation("Попытка установить роль для логина: {Login} роль: {Role}", login, role);

            var user = _context.Users.SingleOrDefault(u => u.Login == login);
            if (user == null)
            {
                _logger.LogWarning("Пользователь с таким логином не найден: {Login}", login);
                return "Пользователь не найден.";
            }

            user.AccessRole = role;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Роль {Role} установлена для пользователя: {Login}", role, login);
            return "Роль выдана успешно.";
        }

        public async Task<List<User>> GetUsers()
        {
            _logger.LogInformation("Попытка получить список пользователей");
            try
            {
                _logger.LogInformation("Пользователи успешно получены");
                return await _context.Users.ToListAsync();
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Ошибка при получении пользователей");
                throw;
            }
        }

        public async Task DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Попытка удалить несуществующего пользователя с id '{id}'", id);
                throw new KeyNotFoundException("Пользователь не найден.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Пользователь с id '{id}' успешно удален", id);
        }
    }
}
