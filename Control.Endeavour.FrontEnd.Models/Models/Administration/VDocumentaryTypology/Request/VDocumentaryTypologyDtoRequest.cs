using Control.Endeavour.FrontEnd.Models.Models.Pagination;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.VDocumentaryTypology.Request
{
    public class VDocumentaryTypologyDtoRequest : PaginationRequest
    {
        public string? ClassCode { get; set; }
        public int DocumentalVersionId { get; set; }

        public int AdministrativeUnitId { get; set; }

        public int ProductionOfficeId { get; set; }

        public int SeriesId { get; set; }

        public int SubSeriesId { get; set; }

        public int DocumentaryTypologyId { get; set; }

        public int DocumentaryTypologyBehaviorId { get; set; }

        public string? TypologyName { get; set; }
        public bool? Radication { get; set; }
    }
}