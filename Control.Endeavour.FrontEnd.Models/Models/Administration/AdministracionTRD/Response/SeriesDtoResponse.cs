namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response
{
    public class SeriesDtoResponse
    {
        public int SeriesId { get; set; }
        public int DocumentalVersionId { get; set; }
        public string DocumentalVersionName { get; set; }
        public int AdministrativeUnitId { get; set; }
        public string AdministrativeUnitName { get; set; }
        public int ProductionOfficeId { get; set; }
        public string ProductionOfficeName { get; set; }
        public string Name { get; set; }
        public string Code { get; set; } = null!;
        public string? Description { get; set; } = null;
        public bool ActiveState { get; set; }
        public string Class { get; set; }
        public string SecurityLevel { get; set; }
        public string CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public RetentionSSDtoResponse? Retention { get; set; } = null;
    }
}