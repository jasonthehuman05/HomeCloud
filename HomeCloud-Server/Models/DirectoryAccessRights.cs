using MySqlToolkit;

namespace HomeCloud_Server.Models
{
    public class DirectoryAccessRights
    {
        [MySqlColumn("DirectoryAccessRightsID", true)]
        public ulong DirectoryAccessRightsID { get; set; }

        [MySqlColumn("UserID", true)]
        public ulong UserID { get; set; }

        [MySqlColumn("DirectoryID", true)]
        public ulong DirectoryID { get; set; }

        [MySqlColumn("CanCreate", true)]
        public bool CanCreate { get; set; }

        [MySqlColumn("CanView", true)]
        public bool CanView { get; set; }

        [MySqlColumn("CanEdit", true)]
        public bool CanEdit { get; set; }

        [MySqlColumn("CanDelete", true)]
        public bool CanDelete { get; set; }
    }
}
