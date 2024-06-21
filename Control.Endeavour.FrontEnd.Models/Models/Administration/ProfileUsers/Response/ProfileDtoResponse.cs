
namespace Control.Endeavour.FrontEnd.Models.Models.Administration.ProfileUsers.Response
{
    public class ProfileDtoResponse
    {
        public int ProfileId { get; set; }
        public int CompanyId { get; set; }
        public string ProfileCode { get; set; } = null!;
        public string Profile1 { get; set; } = null!;
        public string? Description { get; set; }
        public bool ActiveState { get; set; }
        public bool Active { get; set; }
        public string? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}