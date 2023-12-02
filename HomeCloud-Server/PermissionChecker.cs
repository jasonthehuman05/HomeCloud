using HomeCloud_Server.Models;
using HomeCloud_Server.Services;
using System.Diagnostics;
using System.IO;

namespace HomeCloud_Server
{
    static class PermissionChecker
    {
        public static bool AllowedToCreate(uint DirectoryID, int userID, DatabaseService _db)
        {
            //----Check dir to see if user is owner
            //get dir
            Models.Directory directory = _db.GetDirectory(DirectoryID);
            if(directory.OwnerID == userID) //Is owner?
            {
                return true;
            }
            //Check permissions for the directory and user
            List<Models.DirectoryAccessRights> perms = _db.GetUserDirectoryPermissions(DirectoryID, userID);
            if(perms.Count > 0)
            {
                return perms[0].CanCreate;
            }
            return false;
        }

        internal static bool AllowedToDelete(uint DirectoryID, int userID, DatabaseService _db)
        {
            Models.Directory directory = _db.GetDirectory(DirectoryID);
            if (directory.OwnerID == userID) //Is owner?
            {
                return true;
            }
            //Check permissions for the directory and user
            List<Models.DirectoryAccessRights> perms = _db.GetUserDirectoryPermissions(DirectoryID, userID);
            if (perms.Count > 0)
            {
                return perms[0].CanDelete;
            }
            return false;
        }

        internal static bool AllowedToEdit(uint DirectoryID, int userID, DatabaseService _db)
        {
            Models.Directory directory = _db.GetDirectory(DirectoryID);
            if (directory.OwnerID == userID) //Is owner?
            {
                return true;
            }
            //Check permissions for the directory and user
            List<Models.DirectoryAccessRights> perms = _db.GetUserDirectoryPermissions(DirectoryID, userID);
            if (perms.Count > 0)
            {
                return perms[0].CanEdit;
            }
            return false;
        }

        internal static bool AllowedToView(User user, uint DirectoryID, DatabaseService _db)
        {
            int userID = user.UserID;
            Models.Directory directory = _db.GetDirectory(DirectoryID);
            if (directory.OwnerID == userID) //Is owner?
            {
                return true;
            }
            //Check permissions for the directory and user
            List<Models.DirectoryAccessRights> perms = _db.GetUserDirectoryPermissions(DirectoryID, userID);
            if (perms.Count > 0)
            {
                return perms[0].CanView;
            }
            return false;
        }
    }
}