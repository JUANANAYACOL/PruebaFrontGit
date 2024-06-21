using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdParty.Request
{
    public class ThirdPartyUpdateDtoRequest
    {
        public int ThirdPartyId { get; set; }
        public string IdentificationType { get; set; } = null!;
        public string IdentificationNumber { get; set; } = null!;
        public string Names { get; set; } = null!;
        public string? Login { get; set; }
        public string? Password { get; set; }
        public bool ActiveState { get; set; }
        public string? Email1 { get; set; }
        public string? Email2 { get; set; }
        public string? WebPage { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Charge { get; set; }
        public string? Initials { get; set; }
        public string? NatureCode { get; set; }
        public string? Phone1 { get; set; }
        public string? Phone2 { get; set; }
        public virtual AddressDtoRequest? Address { get; set; }
    }
}