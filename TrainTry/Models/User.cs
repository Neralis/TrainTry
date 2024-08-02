namespace TrainTry.Models
{
    //Host=localhost;Port=5432;Database=TrainTry;Username=postgres;Password=123
    //"Host=neralis.ddns.net;Port=5432;Database=TrainTry;Username=mollysss;Password=9001"
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public string AccessRole { get; set; }

        public string IV { get; set; } 
        public User()
        {
            AccessRole = "reader";
        }
    }
    public class UserRegAndLogin
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
    public class UserSetRole
    {
        public string Login { get; set; }
        public string AccessRole { get; set; }
    }
}
