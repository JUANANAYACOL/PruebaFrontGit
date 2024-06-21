#nullable enable

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.ProfileUsers.Request
{
    public class ProfileCreateDtoRequest
    {
        public string Profile1 { get; set; } = null!;
        public string ProfileCode { get; set; } = null!;
        public int CompanyId { get; set; }
        public string? Description { get; set; }
        public bool ActiveState { get; set; }

        public string? User { get; set; }
    }
}