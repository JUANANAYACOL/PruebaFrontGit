

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
public class ManagementTrayDtoRequest
{    
    public int ControlId { get; set; }
    
    public int ActionId { get; set; }
    public string? CommentaryClosed { get; set; }
    public List<AssignedUserIdDtoRequest>? AssignedUserIds { get; set; } = new();     
}

public class AssignedUserIdDtoRequest
{
    public int AssignUserId { get; set; }
    public string Commentary { get; set; }
    public int  InstructionId { get; set; }
    public bool  ItsCopy { get; set; }
}

public class MassiveProcess
{
    public List<ManagementTrayDtoRequest> Documents { get; set; } = new();
}




