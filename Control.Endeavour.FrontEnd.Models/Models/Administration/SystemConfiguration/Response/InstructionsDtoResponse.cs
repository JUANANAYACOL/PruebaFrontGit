

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response
{
    public class InstructionsDtoResponse
    {
        public int? InstructionId { get; set; }

        public string? Name { get; set; } = null!;

        public string? ClassCode { get; set; } = null!;

        public string? ClassCodeName { get; set; } = null!;

        public bool? Active { get; set; }
        public bool ActiveState { get; set; } = true;

        public string? CreateUser { get; set; }

        public string? UpdateUser { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
