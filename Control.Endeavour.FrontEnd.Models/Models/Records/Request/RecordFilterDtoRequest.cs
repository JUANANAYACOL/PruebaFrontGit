using Control.Endeavour.FrontEnd.Models.Models.Pagination;

namespace Control.Endeavour.FrontEnd.Models.Models.Records.Request
{
    public class RecordFilterDtoRequest : PaginationRequest
    {
        public string? RecordId { get; set; }
        public string? recordNumber { get; set; }
        public string? RecordFileType { get; set; }
        public string? RecordType { get; set; }
        public string? RecordName { get; set; }
        public bool? ActiveState { get; set; } = true;
        public int? SubSeriesId { get; set; }
        public int? SeriesId { get; set; }
        public int? ProductionOfficeId { get; set; }
        public int? AdministrativeUnitId { get; set; }
        public int? DocumentalVersionId { get; set; }
        public List<MetaDataFilter> MetaData { get; set; } = new();
    }
    public class MetaDataFilter
    {
        public int MetaFieldId { get; set; }
        public string? DataText { get; set; }
    }

}
