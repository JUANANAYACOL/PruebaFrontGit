using Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Response;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Profile.Request
{
#nullable enable

    public class CreateProfileDtoRequest
    {
        public string? ProfileCode { get; set; }

        public string Profile1 { get; set; } = null!;
        public List<FunctionalityToJson> PermissionJsonClass { get; set; } = null!;

        public string? Description { get; set; }

        public bool ActiveState { get; set; }
    }

    public class FunctionalityToJson
    {
        public int ViewId { get; set; }
        public string? Name { get; set; }

        public bool Create { get; set; } = false;
        public bool Update { get; set; } = false;
        public bool Read { get; set; } = false;
        public bool Delete { get; set; } = false;
        public bool Print { get; set; } = false;
        public bool Append { get; set; } = false;

        public Jsonability Jsonability { get; set; } = null!;
    }

    public class Jsonability
    {
        public bool CreateAbility { get; set; } = false;
        public bool UpdateAbility { get; set; } = false;
        public bool ReadAbility { get; set; } = false;
        public bool DeleteAbility { get; set; } = false;
        public bool PrintAbility { get; set; } = false;
        public bool AppendAbility { get; set; } = false;
    }
}