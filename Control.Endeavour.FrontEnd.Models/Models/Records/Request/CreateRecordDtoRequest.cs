namespace Control.Endeavour.FrontEnd.Models.Models.Records.Request
{
    public class CreateRecordDtoRequest
    {
        public string? RecordName { get; set; }
        public int DocumentalVersionId { get; set; }
        public int? SeriesId { get; set; }
        public int? SubSeriesId { get; set; }
        public string? RecordFileType { get; set; }
        public string? RecordType { get; set; }
        public int Volume { get; set; }
        public string? User { get; set; }
        public List<MetaDataDtoRequest> MetaData { get; set; } = new();

    }
    public class MetaDataDtoRequest
    {
        public int MetaFieldId { get; set; }
        public string? DataText { get; set; }
        public string? ColorData { get; set; }
        
    }
}
