using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaValues.Response;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.VMetaField.Response
{
    public class VMetaFieldDtoResponse
    {
        public int MetaTitleId { get; set; }

        public int? SubSeriesId { get; set; }

        public string? SubSeriesName { get; set; }

        public int? DocumentaryTypologyBagId { get; set; }

        public string? TypologyName { get; set; }

        public string NameRelation { get; set; } = null!;

        public int MetaFieldId { get; set; }

        public string NameMetaField { get; set; } = null!;

        public string FieldType { get; set; } = null!;

        public string? FieldName { get; set; }

        public string Code { get; set; } = null!;

        public bool? Mandatory { get; set; }

        public bool? Anonymization { get; set; }

        public bool? TopographicLocation { get; set; }

        public bool ActiveState { get; set; }

        public string? CreateUser { get; set; }

        public DateTime? CreateDate { get; set; }

        public string? UpdateUser { get; set; }

        public DateTime? UpdateDate { get; set; }

        public List<MetaValuesDtoResponse>? MetaValues { get; set; }
    }
}