using HomeCloud_Server.Services;
using System.Diagnostics;

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
                Debug.WriteLine(perms[0].CanCreate);
                return perms[0].CanCreate;
            }
            return false;
        }
    }
}