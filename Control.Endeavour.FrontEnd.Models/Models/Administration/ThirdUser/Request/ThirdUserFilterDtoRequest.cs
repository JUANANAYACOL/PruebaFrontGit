using Control.Endeavour.FrontEnd.Models.Models.Pagination;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Request
{
    public class ThirdUserFilterDtoRequest : PaginationRequest
    {
        public int ThirdPartyId { get; set; }
        public string IdentificationType { get; set; }
        public string IdentificationNumber { get; set; }
        public string? CellPhone { get; set; }
        public bool ActiveState { get; set; }
        public string Names { get; set; }
        public string Email { get; set; }
    }
}