namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Response
{
    public class PermissionDtoResponse
    {
        public int PermissionId { get; set; }
        public int FunctionalityId { get; set; }
        public string FunctionalityName { get; set; }
        public int ProfileId { get; set; }
        public bool AccessF { get; set; }
        public bool CreateF { get; set; }
        public bool ModifyF { get; set; }
        public bool ConsultF { get; set; }
        public bool DeleteF { get; set; }
        public bool PrintF { get; set; }
        public bool ActiveState { get; set; }
    }
}
