using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaValues.Response;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.MetaField.Response
{
    public class MetaFieldsDtoResponse
    {
        public int MetaFieldId { get; set; }

        public string Code { get; set; } = null!;

        public string NameMetaField { get; set; } = null!;

        public string FieldType { get; set; } = null!;
        public string FieldTypeValue { get; set; } = null!;

        public bool? Anonymization { get; set; }

        public bool? TopographicLocation { get; set; }

        public bool? Mandatory { get; set; }

        public bool ActiveState { get; set; }

        public bool Active { get; set; }
        public string? CreateUser { get; set; }

        public DateTime? CreateDate { get; set; }
        public string? UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }

        public List<MetaValuesDtoResponse>? MetaValues { get; set; }
    }
}