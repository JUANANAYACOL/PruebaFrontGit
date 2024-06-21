namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request
{
    public class DocTemplateDtoRequest
    {
        public int TemplateId { get; set; }
        public string TempCode { get; set; }
        public string TempName { get; set; }
        public string TempType { get; set; }
        public int TempVersion { get; set; }
        public string Process { get; set; }
        public bool ActiveState { get; set; }
        public byte[]? Archivo { get; set; }
        public int CompanyId { get; set; }
        public string User { get; set; }
    }
}