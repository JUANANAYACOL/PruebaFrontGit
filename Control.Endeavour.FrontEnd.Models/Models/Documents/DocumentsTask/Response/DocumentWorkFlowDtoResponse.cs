namespace Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response
{
    public class DocumentWorkFlowDtoResponse
    {
#nullable enable
        public int ManagementOrder { get; set; }

        public int UserTaskId { get; set; }

        public bool indicted { get; set; }

        public string UserTaskName { get; set; } = null!;

        public string? AdministrativeUnitName { get; set; }

        public string? OfficeProductionName { get; set; }

        public string? ChargeName { get; set; }

        public int? UserForwardId { get; set; }

        public string? UserForwardName { get; set; }

        public string ProcessCode { get; set; } = null!;

        public string? Process { get; set; }

        public string? InstructionCode { get; set; }

        public string? Instruction { get; set; }

        public string? Comentary { get; set; }

        public DateTime CreateDate { get; set; }

        public string? TaskDate { get; set; }

        public int? FileId { get; set; }

        public int? FilePdfId { get; set; }
    }
}