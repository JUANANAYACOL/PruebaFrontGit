

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request
{
    public class EditMetadaDocumentDtoRequest
    {
        public int ControlId { get; set; }

        public int DocumentManagementId { get; set; }

        public string? DocDescription { get; set; }

        public string? DocumentId { get; set; }

        public string? NRoGuia { get; set; }

        public string? PriorityCode { get; set; }

        public string? ReceptionCode { get; set; }

        public string? Notification { get; set; }
        public string? SignatoryName { get; set; }

        public int? SignatoryId { get; set; }

        public string? SignatoryType { get; set; }

        public string? User { get; set; }
        public string Justification { get; set; } = null!;
    }
}
