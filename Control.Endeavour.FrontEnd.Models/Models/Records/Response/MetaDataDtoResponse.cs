using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Records.Response
{
    public class MetaDataDtoResponse
    {
        public int? MetaDataId { get; set; }
        public int? MetaFieldId { get; set; }
        public string? NameMetaField { get; set; }
        public string? Color { get; set; }
        public string? MetaValue { get; set; }
        public string? FieldTypeCode { get; set; }
        public string? FieldTypeName { get; set; }
        public int? SeriesId { get; set; }
        public int? SubSeriesId { get; set; }
    }
}
