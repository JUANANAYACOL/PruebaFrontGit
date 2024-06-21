namespace Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response
{
    public class DocumentaryTaskDtoResponse
    {
        public int DocumentaryTaskId { get; set; }
        public int TaskId { get; set; }
        public int? ControlId { get; set; }
        public int? RecordId { get; set; }
        public string ClassCode { get; set; } = null!;
        public int DocumentaryTypologyBehaviorId { get; set; }
        public int DestinationId { get; set; }
        public string DestinationType { get; set; } = null!;
        public int TemplateId { get; set; }
        public string? TaskDescription { get; set; }
        public string? DocDescription { get; set; }
        public DateTime TaskDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int? CreateUserId { get; set; }
        public bool Active { get; set; }
    }
}
