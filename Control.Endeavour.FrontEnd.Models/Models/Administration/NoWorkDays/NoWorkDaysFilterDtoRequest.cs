using Control.Endeavour.FrontEnd.Models.Models.Pagination;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.NoWorkDays
{
    public class NoWorkDaysFilterDtoRequest : PaginationRequest
    {
        public string? Reason { get; set; }
        public DateTime? StarDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;
    }
}
