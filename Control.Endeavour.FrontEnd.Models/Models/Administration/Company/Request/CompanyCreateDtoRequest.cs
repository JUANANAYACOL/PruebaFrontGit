using Control.Endeavour.FrontEnd.Models.Models.Pagination;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Request
{
    public class CompanyCreateDtoRequest : PaginationRequest
    {
        public string IdentificationType { get; set; } = null!;
        public string Identification { get; set; } = null!;
        public string? BusinessName { get; set; }
        public virtual CompanyDataDtoRequest? CompanyData { get; set; }
    }

}
