using System.Diagnostics;

namespace HomeCloud_Server.Services
{
    public class ConfigurationService
    {
        public string FileHostingPath { get; set; }

        public string GetAbsoluteFilePath(string relativePath)
        {
            string path = FileHostingPath + relativePath;
            Debug.WriteLine(path);
            return path;
        }
    }
}
