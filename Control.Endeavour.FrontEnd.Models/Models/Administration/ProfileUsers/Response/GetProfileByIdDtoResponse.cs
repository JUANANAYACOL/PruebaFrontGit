#nullable enable

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.ProfileUsers.Response
{
    public class GetProfileByIdDtoResponse
    {
        public int ProfileId { get; set; }
        public string Profile1 { get; set; } = null!;
        public string? Description { get; set; }
        public bool ActiveState { get; set; }
        public bool Active { get; set; }
    }
}