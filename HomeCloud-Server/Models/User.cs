using MySqlToolkit;

namespace HomeCloud_Server.Models
{
    public class User
    {
        [MySqlColumn("UserID", true)]
        public int UserID { get; set; }

        [MySqlColumn("Username", false)]
        public string Username { get; set;}

        [MySqlColumn("Password", false)]
        public string Password { get; set;}
    }
}
