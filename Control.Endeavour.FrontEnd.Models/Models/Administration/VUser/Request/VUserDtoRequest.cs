using Control.Endeavour.FrontEnd.Models.Models.Pagination;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Request
{
    public class VUserDtoRequest : PaginationRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public int UserId { get; set; }
        public int AdministrativeUnitId { get; set; }
        public int ProductionOfficeId { get; set; }
        public string? ChargeCode { get; set; }

        public bool UserActiveState { get; set; } 

    }
}