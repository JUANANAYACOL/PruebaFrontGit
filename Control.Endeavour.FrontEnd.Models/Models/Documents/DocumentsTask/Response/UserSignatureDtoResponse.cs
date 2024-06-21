namespace Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response
{
    public class UserSignatureDtoResponse
    {
#nullable enable

        public int UserSignatureId { get; set; }

        public int? FileId { get; set; }

        public string SignatureType { get; set; } = null!;

        public string SignatureName { get; set; } = null!;

        public string? SignatureFunction { get; set; }

        public string? SignatureDescription { get; set; }
    }
}
