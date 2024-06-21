namespace Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Request
{
    public class SignatureDtoRequest
    {
        public int SignUserId { get; set; }
        public string? SignatureType { get; set; }
        public DateTime SignatureDate { get; set; }
    }
}
