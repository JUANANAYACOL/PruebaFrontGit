
namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
public class GeneralInformationDtoResponse
{

    public DocumentClasificationDtoResponse? DocumentClasification { get; set; }

    public DocumentInformation? DocumentInformation { get; set; }
}

public class DocumentClasificationDtoResponse
{
    public string? DocumentalVersionCode { get; set; }
    public string? DocumentalVersionName { get; set; }
    public string? AdministrativeUnitCode { get; set; }
    public string? AdministrativeUnitName { get; set; }
    public string? ProductionOfficeCode { get; set; }
    public string? ProductionOfficeName { get; set; }
    public string? SeriesCode { get; set; }
    public string? SeriesName { get; set; }
    public string? SubseriesCode { get; set; }
    public string? SubseriesName { get; set; }
    public string? DocumentaryTypologiesName { get; set; }
    public string? CorrespondenceType { get; set; } = null!;
    public string? LeadManagerName { get; set; }
}

public class DocumentInformation
{
    public int? DocumentId { get; set; }
    public string? ExternalFiling { get; set; }
    public string? ClassCode { get; set; }
    public int? ControlId { get; set; }
    public string? Year { get; set; }
    public string? Priority { get; set; }
    public string? NRoGuia { get; set; }
    public string? ReceptionCode { get; set; }
    public string? DocDescription { get; set; }
    public string? Notificacion { get; set; }
    public string? Firmantes { get; set; }
    public string? Destinatarios { get; set; }
    public DateTime? DueDate { get; set; }
    public string? DaysHoursDueDate { get; set; }
    public bool? Active { get; set; }
    public DateTime? DocDate { get; set; }
    public DateTime? CreateDate { get; set; }
    public string? CreateUser { get; set; }
    public string? BrachOfficeUser { get; set; }
    public string? UserClosedProcess { get; set; }
    public string? CommentaryUserClosedProcess { get; set; }
    public string? ImageVerified { get; set; }
    public string? UserDigitalizedName { get; set; }
    public DateTime? DigitalizedDate { get; set; }



}
