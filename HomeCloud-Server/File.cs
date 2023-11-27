namespace HomeCloud_Server
{
    public class File
    {
        public int FileID { get; set; }
        public string FileName { get; set; }
        public string MIMEType { get; set; }
        public DateTime CreatedOn { get; set; }
        public string PathToData { get; set; }
    }
}
