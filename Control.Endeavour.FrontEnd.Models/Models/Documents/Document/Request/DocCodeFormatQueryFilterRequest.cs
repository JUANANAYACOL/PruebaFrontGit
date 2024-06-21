using Control.Endeavour.FrontEnd.Models.Models.Pagination;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request
{
    public class DocCodeFormatQueryFilterRequest : PaginationRequest
    {
        public string? ClassCode { get; set; }

        public string? Data { get; set; }

        public string? Prefix { get; set; }

        public string? Separator { get; set; }

        public int DataLength { get; set; }

        public string? Suffix { get; set; }
    }
}
