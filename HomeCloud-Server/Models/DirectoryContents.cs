namespace HomeCloud_Server.Models
{
    public class DirectoryContents
    {
        public List<Models.Directory> Directories { get; set; }
        public List<Models.File> Files { get; set; }
    }
}