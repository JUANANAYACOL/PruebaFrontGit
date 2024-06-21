using System.Collections.Generic;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.ProfileUsers.Request
{
    public class ProfilesUserDtoRequest
    {
#nullable enable
        public List<int> ProfileId { get; set; } = null!;

        public int UserId { get; set; }

        public string? User { get; set; }
    }
}