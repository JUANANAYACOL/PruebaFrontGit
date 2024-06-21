namespace Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response
{
    public class ThirdPartyDtoResponse
    {
        public int ThirdPartyId { get; set; }
        public int CompanyId { get; set; }
        public int AddressId { get; set; }
        public string Country { get; set; } = null!;
        public string StateC { get; set; } = null!;
        public string City { get; set; } = null!;

        public string PersonType { get; set; } = null!;

        public string IdentificationType { get; set; } = null!;
        public string IdentificationTypeName { get; set; } = null!;

        public string IdentificationNumber { get; set; } = null!;

        public string Names { get; set; } = null!;

        public bool ActiveState { get; set; }

        public string? Address { get; set; }

        public string? FullName { get; set; }

        public string? FirstName { get; set; }

        public string? MiddleName { get; set; }

        public string? LastName { get; set; }

        public string? Charge { get; set; }

        public string? Initials { get; set; }

        public string NatureName { get; set; } = null!;
        public string? NatureCode { get; set; }

        public string? Phone1 { get; set; }

        public string? Phone2 { get; set; }

        public string? Email1 { get; set; }

        public string? Email2 { get; set; }

        public string? WebPage { get; set; }

        public List<ThirdUserDtoResponse>? ThirdUsers { get; set; }

        public string? CreateUser { get; set; }

        public string? UpdateUser { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public bool Selected { get; set; }
        public bool EnableSelection { get; set; } = true;
        public bool EnableCopy { get; set; } = true;
        public bool Copy { get; set; }
    }
}