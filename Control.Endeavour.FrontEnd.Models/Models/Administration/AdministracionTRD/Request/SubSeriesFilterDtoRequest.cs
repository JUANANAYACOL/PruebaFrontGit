

using Control.Endeavour.FrontEnd.Models.Models.Pagination;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request
{
    public class SubSeriesFilterDtoRequest : PaginationRequest
    {
        public int SubSeriesId { get; set; }
        public int SeriesId { get; set; }
        public int ProductionOfficeId { get; set; }
        public int AdministrativeUnitId { get; set; }
        public int DocumentalVersionId { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
    }
}
