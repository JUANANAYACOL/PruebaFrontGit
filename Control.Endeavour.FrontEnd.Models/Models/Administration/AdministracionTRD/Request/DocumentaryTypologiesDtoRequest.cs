namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request
{
    public class DocumentaryTypologiesDtoRequest
    {
        public int DocumentaryTypologyBagId { get; set; }

        public int? SeriesId { get; set; }
        public int? SubSeriesId { get; set; }
        public bool ActiveState { get; set; }
        public string? SecurityLevel { get; set; }
        public int? ProductionOfficeId { get; set; }
        public List<int>? UsersId { get; set; }
    }
}