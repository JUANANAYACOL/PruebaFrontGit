namespace Control.Endeavour.FrontEnd.Models.Models.Administration.ImporterTrdTvd.Response
{
    public class ImporterHistoryDtoResponse
    {
        public int ImporterHistoryId { get; set; }
        public int DocumentalVersionId { get; set; }
        public string DocumentalVersionName { get; set; }
        public int FileId { get; set; }
        public int? AdministrativeUnitsCount { get; set; }
        public int? ProductionOfficesCount { get; set; }
        public int? SeriesCount { get; set; }
        public int? SubSeriesCount { get; set; }
        public int? RetentionsCount { get; set; }
        public int? DocumentaryTypologiesBagCount { get; set; }
        public int? TRDCount { get; set; }
        public int? TRDCCount { get; set; }
        public string DescriptionHistory { get; set; } = null;
        public string CreateUser { get; set; } = null;
        public DateTime CreateDate { get; set; }
        public string RecordsImported { get; set; }
    }
}