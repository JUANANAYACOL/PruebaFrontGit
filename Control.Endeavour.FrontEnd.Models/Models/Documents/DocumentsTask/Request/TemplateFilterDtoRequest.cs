using Control.Endeavour.FrontEnd.Models.Models.Pagination;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Request
{
    public class TemplateFilterDtoRequest : PaginationRequest
    {
#nullable enable
        public string? Code { get; set; }
        public string? NameTemplate { get; set; }
        public int Version { get; set; }
        public string? Type { get; set; }
        public string? Process { get; set; }
        public bool? ValidateActiveState { get; set; }
    }
}