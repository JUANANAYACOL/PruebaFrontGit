using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;

public class ObjectTransaction
{
    public RetunUserSearch UserInfo { get; set; }
    public int InstructionId { get; set; }
    public string? Days { get; set; }
    public string? Hours { get; set; }
    public string? Subject { get; set; }
    public int Position { get; set; }
    public int CountCharacters { get; set; } = 0;
}
