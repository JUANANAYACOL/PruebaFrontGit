

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.ProfileUsers.Request
{
    public class CreateAssignProfile
    {
        public List<int>? ProfileId { get; set; }
        public int UserId { get; set; }
        public string? User { get; set; }
    }
}
