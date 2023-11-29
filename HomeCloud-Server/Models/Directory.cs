using MySqlToolkit;

namespace HomeCloud_Server.Models
{
    public class Directory
    {
        [MySqlColumn("DirectoryID", true)]
        public int DirectoryID { get; set; }

        [MySqlColumn("ParentDirectoryID", false)]
        public uint ParentDirectory { get; set; }

        [MySqlColumn("DirName", false)]
        public string DirectoryName { get; set; }
    }
}
