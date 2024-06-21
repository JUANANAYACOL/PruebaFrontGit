

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Request
{
    public class UserUpdateDtoRequest
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public int ProfileId { get; set; }
        public int ProductionOfficeId { get; set; }

        public int BranchOfficeId { get; set; }

        public bool OriginAd { get; set; }
        public bool ActiveState { get; set; }

        public string FirstName { get; set; } = null!;

        public string? MiddleName { get; set; }

        public string LastName { get; set; } = null!;

        public string? IdentificationType { get; set; }

        public string Identification { get; set; } = null!;

        public DateTime BirthDate { get; set; }

        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }

        public string? CellPhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? ChargeCode { get; set; }

        public string? ContractType { get; set; }

        public string? ContractNumber { get; set; }

        public DateTime? ContractStartDate { get; set; }

        public DateTime? ContractFinishDate { get; set; }
    }
}
