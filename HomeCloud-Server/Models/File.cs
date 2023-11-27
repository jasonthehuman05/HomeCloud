using MySqlToolkit;
namespace HomeCloud_Server.Models
{
    public class File
    {
        [MySqlColumn("FileID", true)]
        public int FileID { get; set; }

        [MySqlColumn("FileName", false)]
        public string FileName { get; set; }

        [MySqlColumn("MimeType", false)]
        public string MIMEType { get; set; }

        [MySqlColumn("CreatedOn", false)]
        public ulong CreatedOnTimestamp { get; set; }

        [MySqlColumn("PathToData", false)]
        public string PathToData { get; set; }
    }
}
