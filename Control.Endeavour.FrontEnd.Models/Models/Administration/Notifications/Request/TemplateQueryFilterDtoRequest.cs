using Control.Endeavour.FrontEnd.Models.Models.Pagination;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Notifications.Request
{
    public class TemplateQueryFilterDtoRequest : PaginationRequest
    {
        public int? CompanyId { get; set; }

        public string? NameTemplate { get; set; }

        public int? TypeId { get; set; }
    }
}
