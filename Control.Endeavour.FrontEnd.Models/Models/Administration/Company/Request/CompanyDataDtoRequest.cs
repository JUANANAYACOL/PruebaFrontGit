using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Request
{
    public class CompanyDataDtoRequest
    {
        public string? LegalAgentIdType { get; set; }
        public string? LegalAgentId { get; set; }
        public string? LegalAgentFullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CellPhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? WebAddress { get; set; }
        public string? Domain { get; set; }
        public FileCompanyDtoRequest? LogoFile { get; set; }
        public FileCompanyDtoRequest? BannerFile { get; set; }
        public FileCompanyDtoRequest? MobileBannerFile { get; set; }

        public virtual AddressDtoRequest Address { get; set; } = null!;
    }
}
