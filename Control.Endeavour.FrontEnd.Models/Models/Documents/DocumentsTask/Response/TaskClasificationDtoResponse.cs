namespace Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response
{
    public class TaskClasificationDtoResponse
    {
#nullable enable
        public string ClassCode { get; set; } = null!;
        public string ComunicationClass { get; set; } = null!;
        public string? ShipingMethod { get; set; }
        public string? ReceptionCode { get; set; }
        public string TypologyName { get; set; } = null!;
        public string? AdministrativeUnitName { get; set; }
        public string? ProductionOfficeName { get; set; }
        public string? SeriesName { get; set; }
        public string? SubSeriesName { get; set; }
        public List<UserClasification>? DestinationsUser { get; set; }
        public List<AdministrationUsers>? DestinationsAdministration { get; set; }

    }

    public class UserClasification
    {
        public int UserId { get; set; }
        public int? TaskId { get; set; }
        public string? Type { get; set; }
        public string? FullName { get; set; }
        public string? AdministrativeUnitName { get; set; }
        public string? ProductionOfficeName { get; set; }
        public string? Charge { get; set; }

    }


    public class AdministrationUsers
    {
        public int? ThirdPartyId { get; set; }
        public int? ThirdUserId { get; set; }
        public int? TaskId { get; set; }
        public string? Type { get; set; }
        public string? CompanyName { get; set; }
        public string? IdentificationTypeName { get; set; }
        public string? IdentificationNumber { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }

    }
}
