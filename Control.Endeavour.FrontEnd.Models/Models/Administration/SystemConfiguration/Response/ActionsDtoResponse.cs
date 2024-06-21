

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response
{
    public class ActionsDtoResponse
    {
        public int? ActionId { get; set; }

        public string Name { get; set; } = null!;

        public string FlowStateCode { get; set; } = null!;

        public string FlowStateName { get; set; } = null!;

        public string ClassCode { get; set; } = null!;

        public string ClassCodeName { get; set; } = null!;

        public string Origin { get; set; } = null!;

        public string OriginName { get; set; } = null!;

        public bool Active { get; set; }
        public bool ActiveState { get; set; } = true;

        public string? CreateUser { get; set; }

        public string? UpdateUser { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
