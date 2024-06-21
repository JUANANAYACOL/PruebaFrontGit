using Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Response;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Profile.Request
{
#nullable enable

    public class UpdateProfileDtoRequest
    {
        public int ProfileId { get; set; }

        public string? ProfileCode { get; set; }

        public string Profile1 { get; set; } = null!;

        public string? Description { get; set; }

        public List<FunctionalityToJson> PermissionJsonClass { get; set; } = null!;

        public bool ActiveState { get; set; }
    }
}