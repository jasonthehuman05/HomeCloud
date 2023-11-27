namespace HomeCloud_Server.Models
{
    public class DatabaseSettings
    {
        public string   username    { get; set; } = null!;
        public string   password    { get; set; } = null!;
        public string   databaseName{ get; set; } = null!;
        public string   ipAddress   { get; set; } = null!;
        public int      port        { get; set; }
    }
}
