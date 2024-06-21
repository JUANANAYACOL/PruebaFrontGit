using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaValues.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.MetaData.Request
{
    public class MetaDataRelationDtoRequest
    {
        public int MetaFieldId { get; set; }

        public string DataText { get; set; } = null!;
        public int OrderData { get; set; }
        public string ColorData { get; set; } = null!;

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

        public List<MetaValuesDtoResponse>? MetaValues { get; set; }
    }
}