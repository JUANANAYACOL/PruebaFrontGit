using Control.Endeavour.FrontEnd.Models.Models.Pagination;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Request
{
    public class FilterManagementDtoRequest : PaginationRequest
    {
#nullable enable

        public int TaskId { get; set; }

        public int ControlId { get; set; }

        public string? ClassCode { get; set; }

        public int? UserId { get; set; }

        public bool UserTaskId { get; set; }

        public bool UserForwardId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public List<string>? ProcessCode { get; set; } = new List<string>();

        public List<string>? InstructionCode { get; set; } = new List<string>();

        public string? TaskDescription { get; set; }

        public bool? Indicted { get; set; }
    }

    public class Visor
    {
        public int identification { get; set; }
        public int id { get; set; }
    }
}
