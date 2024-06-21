using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.BranchOffice.Request
{
    public class BranchOfficeUpdateDtoRequest
    {
        public int BranchOfficeId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string NameOffice { get; set; } = string.Empty;
        public string Region { get; set; }
        public string Territory { get; set; }
        public bool ActiveState { get; set; }

        public AddressDtoRequest? Address { get; set; }
    }
}