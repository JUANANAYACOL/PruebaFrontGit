using Control.Endeavour.FrontEnd.Models.Models.Pagination;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Request
{
    public class ThirdUserDtoRequest
    {
        public int ThirdUserId { get; set; }

        public int ThirdPartyId { get; set; }
        public string IdentificationType { get; set; }

        public string IdentificationNumber { get; set; }
        public string IdentificationTypeName { get; set; }
        public string? CellPhone { get; set; }
        public string? Area { get; set; }

        public string? Charge { get; set; }

        public string Names { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }
        public bool ActiveState { get; set; }

        public string? CreateUser { get; set; }

        public string? UpdateUser { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}