using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Response
{
    public class CompanyDtoResponse
    {
        public int CompanyId { get; set; }
        public string IdentificationType { get; set; } = null!;
        public string IdentificationTypeName { get; set; } = null!;
        public string Identification { get; set; } = null!;
        public string BusinessName { get; set; } = null!;
        public string? LegalAgentIdType { get; set; }
        public string? LegalAgentIdTypeName { get; set; }
        public string? LegalAgentId { get; set; }
        public string? LegalAgentFullName { get; set; }
        public string Address { get; set; } = null!;
        public int AddressId { get; set; }
        public string CountryName { get; set; } = null!;
        public string StateName { get; set; } = null!;
        public string CityName { get; set; } = null!;
        public string? PhoneNumber { get; set; } 
        public string? CellPhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? WebAddress { get; set; } 
        public string? Domain { get; set; }
        public int? LogoFileId { get; set; }
        public int? BannerFileId { get; set; }
        public int? MobileBannerFileId { get; set; }
        public DateTime CreateDate { get; set; }
        public string? CreateUser { get; set; }
    }
}
