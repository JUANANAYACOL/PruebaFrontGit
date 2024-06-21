namespace Control.Endeavour.FrontEnd.Models.Models.Administration.ProfileUsers.Request
{
    public class ProfileByFilterDtoRequest
    {
#nullable enable
        public string? Name { get; set; }

        public string? Description { get; set; }
        public int CompanyId { get; set; }
    }
}