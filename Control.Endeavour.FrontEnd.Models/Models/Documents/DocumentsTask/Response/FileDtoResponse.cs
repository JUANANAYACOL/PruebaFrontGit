namespace Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response
{
    public class FileDtoResponse
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
        public string FileExt { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public bool Active { get; set; }
        public byte[] DataFile { get; set; }
    }
}
