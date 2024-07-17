namespace TrainTry.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public string AccessRole { get; set; }

        public User()
        {
            AccessRole = "reader";
        }
    }
}
