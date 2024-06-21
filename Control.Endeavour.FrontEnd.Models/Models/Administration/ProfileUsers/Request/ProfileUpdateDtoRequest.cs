namespace Control.Endeavour.FrontEnd.Models.Models.Administration.ProfileUsers.Request
{
    public class ProfileUpdateDtoRequest
    {

        public int ProfileId { get; set; }
        public string Profile1 { get; set; } = null!;
        public string? Description { get; set; }
        public bool ActiveState { get; set; }
        public string? User { get; set; }
    }
}