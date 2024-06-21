using Control.Endeavour.FrontEnd.Models.Models.Pagination;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;

public class ManagementTrayFylterDtoRequest : PaginationRequest 
{
    
    public string? FlowStateCode { get; set; }
    public string? ClassCode { get; set; }
    public int? ControlId { get; set; }
    public string? FilingCode { get; set; }
    public string? PriorityCode { get; set; }
    public int? Year { get; set; }
    public int? Month { get; set; }
    public int? Days { get; set; }
    public bool? DueDate { get; set; }
    public bool ItsCopy { get; set; }
    public int AssingUserId { get; set; } = 0;

    public bool IsAnotherUser { get; set; } = false;

}