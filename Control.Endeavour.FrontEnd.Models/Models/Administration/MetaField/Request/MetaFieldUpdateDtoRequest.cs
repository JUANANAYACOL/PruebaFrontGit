using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaValues.Request;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.MetaField.Request
{
    public class MetaFieldUpdateDtoRequest
    {
        public int MetaFieldId { get; set; }
        public bool? Anonymization { get; set; }

        public bool? TopographicLocation { get; set; }

        public bool ActiveState { get; set; }

        public bool? Mandatory { get; set; }

        public string Code { get; set; } = null!;

        public string NameMetaField { get; set; } = null!;

        public string FieldType { get; set; } = null!;

        public string? User { get; set; }

        public bool DeleteMetaValues { get; set; }
        public List<MetaValueCreateDtoRequest>? MetaValues { get; set; }
    }
}