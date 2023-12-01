using Microsoft.Extensions.Primitives;
using MySqlToolkit;

namespace HomeCloud_Server.Models
{
    public class AuthToken
    {
        [MySqlColumn("Token", false)]
        public string Token { get; set; }

        [MySqlColumn("UserID", false)]
        public int UserID { get; set; }

        [MySqlColumn("ExpiryTimestamp", false)]
        public ulong ExpiryTimestamp { get; set; }
    }
}
