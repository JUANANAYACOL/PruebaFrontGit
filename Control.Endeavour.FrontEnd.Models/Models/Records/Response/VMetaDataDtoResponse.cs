namespace Control.Endeavour.FrontEnd.Models.Models.Records.Response
{
    public class VMetaDataDtoResponse
    {
        public int? RecordId { get; set; } = 0;
        public int? MetaFieldId { get; set; }
        public string NameMetaField { get; set; } = null!;
        public string? FieldTypeCode { get; set; } = null!;
        public string? FieldTypeName { get; set; }
        public int? MetaDataId { get; set; } = 0;
        public string Color { get; set; } = null!;
        public string MetaValue { get; set; } = null!;
        public int? SeriesId { get; set; }
        public int? SubSeriesId { get; set; }
        public bool Active { get; set; }
        public bool Mandatory { get; set; }
        public DateTime? MetaDateValue {  get; set; }
        public bool? MetaBoolValue { get; set; }
        public bool IsUpdated { get; set; } = false;
    }
}
