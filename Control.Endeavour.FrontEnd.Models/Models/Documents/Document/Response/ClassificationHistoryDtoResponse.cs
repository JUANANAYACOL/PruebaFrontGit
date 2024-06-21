

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response
{
    public class ClassificationHistoryDtoResponse
    {
        public int ControlId { get; set; }
        public int? DocumentaryTypologyBehaviorLastId { get; set; }
        public int DocumentaryTypologyBehaviorId { get; set; }
        public string? AdministrativeUnitName {  get; set; }
        public string? ProductionOfficeName { get; set; }
        public string? SeriesName { get; set; }
        public string? SubseriesName { get; set; }
        public string? TypologyName { get; set; }
        public string? Justification { get; set; }
        public string? User { get; set; }
        public string? CreateDate { get; set; }
    }
}
