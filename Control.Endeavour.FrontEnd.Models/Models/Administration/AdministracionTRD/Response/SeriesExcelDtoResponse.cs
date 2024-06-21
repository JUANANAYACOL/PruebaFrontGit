namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response
{
    public class SeriesExcelDtoResponse
    {
        public int SeriesId { get; set; }
        public int ProductionOfficeId { get; set; }
        public string ProductionOfficeName { get; set; }
        public string Name { get; set; }
        public string Code { get; set; } = null!;
        public string Description { get; set; }
        public bool? ActiveState { get; set; }
        public int? RetentionId { get; set; }
        public int? TimeFileManagement { get; set; }
        public int? TimeFileCentral { get; set; }
        public bool? TotalConservation { get; set; }
        public bool? Elimination { get; set; }
        public bool? TechEnvironment { get; set; }
        public bool? Selection { get; set; }
        public string? ProcedureRet { get; set; }
        public string CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}