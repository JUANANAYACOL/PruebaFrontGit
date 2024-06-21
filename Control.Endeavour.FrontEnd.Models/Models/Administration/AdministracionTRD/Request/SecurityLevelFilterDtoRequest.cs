using Control.Endeavour.FrontEnd.Models.Models.Pagination;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request
{
    public class SecurityLevelFilterDtoRequest : PaginationRequest
    {
        public int? SeriesId { get; set; }
        public int? SubSeriesId { get; set; }
        public int? DocumentaryTypologyId { get; set; }
        public int UserId { get; set; }
        public string? SecurityLevelCode { get; set; }
    }
}