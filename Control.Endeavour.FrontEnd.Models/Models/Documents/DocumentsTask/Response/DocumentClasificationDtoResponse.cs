using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response
{
    public class DocumentClasificationDtoResponse
    {
        public string? Description { get; set; }
        public int? IdTypology { get; set; }
        public string ClassCode { get; set; } = null!;
        public string ComunicationClass { get; set; } = null!;
        public string? ShipingMethod { get; set; }
        public string? ReceptionCode { get; set; }
        public string? TypologyName { get; set; }
        public string? AdministrativeUnitName { get; set; }
        public string? ProductionOfficeName { get; set; }
        public string? SeriesName { get; set; }
        public string? SubSeriesName { get; set; }
        public List<UserClasification>? DestinationsUser { get; set; }
        public List<AdministrationUsers>? DestinationsAdministration { get; set; }
    }
}
