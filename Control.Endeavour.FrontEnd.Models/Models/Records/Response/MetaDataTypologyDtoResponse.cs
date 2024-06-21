using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Records.Response
{
    public class MetaDataTypologyDtoResponse
    {
        public int MetaDataId { get; set; }
        public int MetaFieldId { get; set; }
        public string NameMetaField { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string MetaValue { get; set; } = null!;
        public string FieldTypeCode { get; set; } = null!;
        public string? FieldTypeName { get; set; }
        public int? DocumentId { get; set; }
        public bool Active { get; set; }
        public bool? Mandatory { get; set; }
    }
}
