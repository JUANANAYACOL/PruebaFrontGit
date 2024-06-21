using Control.Endeavour.FrontEnd.Models.Models.Administration.Profile.Request;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Profile.Response
{
#nullable enable

    public class ProfileDtoResponse
    {
        public int ProfileId { get; set; }

        public int CompanyId { get; set; }

        public string? ProfileCode { get; set; }

        public string Profile1 { get; set; } = null!;

        public string? Description { get; set; }

        public string PermissionsJson { get; set; } = null!;
        public List<FunctionalityToJson> PermissionJsonClass { get; set; } = null!;

        public bool ActiveState { get; set; }

        public bool Active { get; set; }

        public string? CreateUser { get; set; }

        public string? UpdateUser { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}