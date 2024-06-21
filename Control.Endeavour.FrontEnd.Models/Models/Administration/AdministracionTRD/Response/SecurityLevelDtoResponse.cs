namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response
{
    public class SecurityLevelDtoResponse
    {
        public int DocumentaryTypologyId { get; set; }
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string ProductionOfficeName { get; set; }
        public string? SecurityLevelCode { get; set; }
        public string? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool Selected { get; set; } = false;
    }
}