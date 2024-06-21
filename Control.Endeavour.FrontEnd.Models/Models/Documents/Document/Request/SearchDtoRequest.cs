using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Models.Models.Records.Request;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request
{
    public class SearchDtoRequest : PaginationRequest
    {
        public int ControlId { get; set; }
        public string? DocDescription { get; set; }
        public int DocumentaryTypologyBehaviorId { get; set; }
        public string? ClassCode { get; set; }
        public string? FilingCode { get; set; }
        public int UserId { get; set; }
        public bool SearchManagementUser { get; set; }
        public bool SearchCreateDocument { get; set; }
        public bool SearchDocumentCanceled { get; set; }
        public bool SearchFilingCode { get; set; }
        public string? NroDocument { get; set; }
        public string? NroGuia { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<SignatoryDtoRequest> Signatories { get; set; } = new List<SignatoryDtoRequest>();
        public List<ReceiversDtoRequest> Receivers { get; set; } = new List<ReceiversDtoRequest>();
        public List<MetaDataFilter> MetaData { get; set; } = new List<MetaDataFilter>();
    }


    public class SignatoryDtoRequest
    {
        public int IdUser { get; set; }

        public string? TypeUser { get; set; }
    }

    public class ReceiversDtoRequest
    {
        public int IdUser { get; set; }

        public string? TypeUser { get; set; }
    }
}
