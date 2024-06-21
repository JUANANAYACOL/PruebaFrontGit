using Control.Endeavour.FrontEnd.Models.Models.Pagination;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Replacement.Request
{
    public class ReplacementFilterDtoRequest : PaginationRequest
    {
        public int UserId { get; set; }
        public int UserReplacementId { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string ReplacementName { get; set; }
        public DateTime? DateBetween { get; set; }
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}