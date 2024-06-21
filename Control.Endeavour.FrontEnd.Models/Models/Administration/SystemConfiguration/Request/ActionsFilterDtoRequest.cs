

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request
{
    public class ActionsFilterDtoRequest
    {
        public string? Name { get; set; } = null!;

        public string? FlowStateCode { get; set; } = null!;

        public string? ClassCode { get; set; } = null!;

        public string? Origin { get; set; } = null!;
    }
}
