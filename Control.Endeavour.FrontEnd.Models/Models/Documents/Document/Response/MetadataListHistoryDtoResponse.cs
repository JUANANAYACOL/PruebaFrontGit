using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response
{
    public class MetadataListHistoryDtoResponse
    {
        public string Justification { get; set; } = null!;

        public DateTime CreateDate { get; set; }

        public string CreateUser { get; set; } = null!;

        public EditMetadaDocumentDtoRequest MetadatachangeList { get; set; }

    }
    public class SearchMetadatosChangeDtoResponse
    {
        public string? DocDescription { get; set; }

        public string? NRoGuia { get; set; }

        public string? DocumentId { get; set; }

        public string? SignatoryName { get; set; }

        public string? Notification { get; set; }

        public string? ReceptionCode { get; set; }

        public string? PriorityCode { get; set; }


    }
}
