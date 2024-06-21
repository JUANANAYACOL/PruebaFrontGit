namespace Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response
{
    public class DocumentResponseComentaryClosedDtoResponse
    {
#nullable enable
        public int ControlId { get; set; }

        public int ActionId { get; set; }

        public string? FilingCode { get; set; }

        public string? ClassCode { get; set; }

        public string? ShipingMethod { get; set; }

        public int DocumentaryTypologiesBehaviorId { get; set; }

        public string? TypologyName { get; set; }

        public string? AdministrativeUnitName { get; set; }

        public string? ProductionOfficeName { get; set; }

        public string? SeriesName { get; set; }

        public string? SubSeriesName { get; set; }

        public List<AdministrationUsers>? DestinationsAdministration { get; set; } = new List<AdministrationUsers>();
    }
}