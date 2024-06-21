

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Request
{
    public class CreatePermissionDtoRequest
    {
        public int? PermissionId { get; set; }
        public int FunctionalityId { get; set; }
        public string? FunctionalityName { get; set; }
        public int UserId { get; set; }
        public int? ProfileId { get; set; }
        public bool AccessF { get; set; }
        public bool CreateF { get; set; }
        public bool ModifyF { get; set; }
        public bool ConsultF { get; set; }
        public bool DeleteF { get; set; } = false;
        public bool PrintF { get; set; }
        public bool ActiveState { get; set; }
        public string User { get; set; }
    }
}
