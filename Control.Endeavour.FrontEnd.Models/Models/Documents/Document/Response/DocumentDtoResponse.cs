
namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
public class DocumentDtoResponse
{
    public int AssignUserId { get; set; }
    public int ControlId { get; set; }
    public int DocumentManagementId { get; set; }
    public string? NameDocumentaryTypologiesBag { get; set; }
    public string? FilingCode { get; set; }
    public string? DocDescription { get; set; }
    public DateTime DocDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreateDate { get; set; }
    public string? PriorityCode { get; set; }
    public string? PriorityCodeName { get; set; }
    public string? DocumentReceivers { get; set; }
    public string? DocumentSignatories { get; set; }
    public string? ClassCode {  get; set; }
    public bool Selected { get; set; } = false;
    public int? FileId { get; set; }

}


