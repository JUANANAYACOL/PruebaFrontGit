namespace Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response
{
    public class TemplateDocumentDtoResponse
    {
        public int TemplateId { get; set; }
        public string? TempCode { get; set; }
        public string? TempName { get; set; }
        public string? Process { get; set; }
        public int TempVersion { get; set; }
        public int FileId { get; set; }
        public string? TempType { get; set; }
        public bool ActiveState { get; set; }
        public string? CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
