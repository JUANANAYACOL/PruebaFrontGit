using Control.Endeavour.FrontEnd.Models.Models.Pagination;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.MetaField.Request
{
    public class MetaFieldsFilterDtoRequest : PaginationRequest
    {
        public string Code { get; set; } = null!;

        public string NameMetaField { get; set; } = null!;

        public string FieldType { get; set; } = null!;
        public int DocumentaryTypologyBagId { get; set; }

        public int SeriesId { get; set; }

        public int SubSeriesId { get; set; }
    }
}