namespace Control.Endeavour.FrontEnd.Models.Models.Administration.User.Response
{
    public class UserDtoResponse
    {
        public int UserId { get; set; }
        public int ProfileId { get; set; }

        public string? UserName { get; set; }
        public bool ActiveState { get; set; }
        public int CompanyId { get; set; }

        public int ProductionOfficeId { get; set; }

        public int BranchOfficeId { get; set; }

        ///Informacion de USERDATA
        public int? PictureFileId { get; set; }

        public byte[]? ProfilePictureDataFile { get; set; }

        public string? FullName { get; set; }

        public string FirstName { get; set; } = null!;

        public string? MiddleName { get; set; }

        public string LastName { get; set; } = null!;

        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }

        public string? CellPhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? IdentificationType { get; set; }
        public string? IdentificationTypeName { get; set; }

        public string Identification { get; set; } = null!;

        public DateTime BirthDate { get; set; }

        public string? ChargeCode { get; set; }

        public string ChargeName { get; set; } = null!;

        public string? ContractType { get; set; }

        public bool OriginAd { get; set; }
        public string? ContractTypeName { get; set; } = null;
        public string? ContractNumber { get; set; }
    }
}