namespace Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response
{
    public class VDocumentaryTaskDtoResponse
    {
#nullable enable
        public bool? ViewState { get; set; }
        public bool? Indicted { get; set; }
        public int TaskId { get; set; }
        public int TaskManagementId { get; set; }
        public int? ControlId { get; set; }
        public string? Class { get; set; }
        public int? UserTaskId { get; set; }
        public string UserTaskName { get; set; } = null!;
        public int? UserForwardId { get; set; }
        public string? UserForwardName { get; set; }
        public DateTime TaskDate { get; set; }
        public string? TaskDescription { get; set; }
        public string? Instruction { get; set; }
        public string? Process { get; set; }
    }
}
