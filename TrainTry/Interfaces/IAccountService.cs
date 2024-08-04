using TrainTry.Models;

namespace TrainTry.Interfaces
{
    public interface IAccountService
    {
        Task<string> Register(string login, string password);
        string Login(string login, string password);
        Task<string> SetRole(string login, string role);
        Task<List<User>> GetUsers();
        Task DeleteUser(int id);
    }
}
