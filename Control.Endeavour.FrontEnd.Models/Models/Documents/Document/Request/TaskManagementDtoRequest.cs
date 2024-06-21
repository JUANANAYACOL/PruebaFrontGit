namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request
{
    public class TaskManagementDtoRequest
    {
        public byte[]? Archive { get; set; }
        public string? InstructionCode { get; set; } = string.Empty;
        public string ProcessCode { get; set; } = null!;
        public string? Comentary { get; set; } = string.Empty;
        public int UserTaskId { get; set; }
        public int? UserForwardId { get; set; } = 0;
    }
}
