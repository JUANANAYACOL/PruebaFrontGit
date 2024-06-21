

using Control.Endeavour.FrontEnd.Models.Models.Pagination;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Request
{
    public class AdministrativeUnitFilterDtoRequest : PaginationRequest
    {
        public int? DocumentalVersionId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
    }
}