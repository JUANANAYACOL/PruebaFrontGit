namespace Control.Endeavour.FrontEnd.Models.Models.Records.Response
{
    public class VRecordsDtoResponse
    {
        public int RecordId { get; set; }
        public int RecordNumber { get; set; }
        public string? RecordFileType { get; set; }
        public string RecordFileTypeName { get; set; } = null!;
        public string? RecordType { get; set; }
        public string RecordTypeName { get; set; } = null!;
        public bool ActiveState { get; set; }
        public bool Active { get; set; }
        public string? CodeSubSeries { get; set; }
        public string? NameSubSeies { get; set; }
        public string? DescriptionSubSeries { get; set; }
        public int? SubSeriesId { get; set; }
        public int? SeriesId { get; set; }
        public string CodeSeries { get; set; } = null!;
        public string? NameSeries { get; set; }
        public string? DescriptionSeries { get; set; }
        public int ProductionOfficeId { get; set; }
        public string CodeProductionOffices { get; set; } = null!;
        public string? NameProductionOffices { get; set; }
        public string? DescriptionProductionOffices { get; set; }
        public int AdministrativeUnitId { get; set; }
        public string CodeAdministrativeUnits { get; set; } = null!;
        public string? NameAdministrativeUnits { get; set; }
        public string? DescriptionAdministrativeUnits { get; set; }
        public int DocumentalVersionId { get; set; }
        public string? VersionType { get; set; }
        public string? NameDocumentalVersion { get; set; }
        public string? Code { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Volumes { get; set; }
        public string? MetaData { get; set; }
        public string? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
