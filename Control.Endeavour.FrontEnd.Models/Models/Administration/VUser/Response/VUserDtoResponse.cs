namespace Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response
{
    public class VUserDtoResponse
    {
        public int CompanyId { get; set; }

        public int UserId { get; set; }
        public int ProfileId { get; set; }

        public string? CreateUser { get; set; }

        public bool UserActiveState { get; set; }

        public DateTime? CreateDate { get; set; }

        public string? UpdateUser { get; set; }

        public DateTime? UpdateDate { get; set; }
        public string UserName { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string? MiddleName { get; set; }

        public string LastName { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string? IdentificationTypeCode { get; set; }

        public string? IdentificationType { get; set; }

        public string? Identification { get; set; }

        public int AdministrativeUnitId { get; set; }

        public string? AdministrativeUnitCode { get; set; }

        public string? AdministrativeUnitName { get; set; }

        public string AdministrativeUnit { get; set; } = null!;

        public int? ProductionOfficeId { get; set; }

        public string? ProductionOfficeCode { get; set; }

        public string? ProductionOfficeName { get; set; }

        public string ProductionOffice { get; set; } = null!;

        public int? BranchOfficeId { get; set; }

        public string? BranchOfficeCode { get; set; }

        public string? BranchOfficeName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? CellPhoneNumber { get; set; }

        public string? ChargeCode { get; set; }

        public string? Charge { get; set; }

        public string? ContractTypeCode { get; set; }

        public string? ContractType { get; set; }

        public string? ContractNumber { get; set; }

        public DateTime? ContractStartDate { get; set; }

        public DateTime? ContractFinishDate { get; set; }
        public DateTime? BirthDate { get; set; }

        public int? PictureFileId { get; set; }

        public bool Selected { get; set; } = false;
        public bool Copy { get; set; } = false;
        public ProfileUser? UserProfile { get; set; }
        public bool OriginAd { get; set; }

        public static explicit operator List<object>(VUserDtoResponse v)
        {
            throw new NotImplementedException();
        }
    }
    public class ProfileUser
    {
        public byte[]? DataFile { get; set; }
        public string? FileExt { get; set; }
    }
}