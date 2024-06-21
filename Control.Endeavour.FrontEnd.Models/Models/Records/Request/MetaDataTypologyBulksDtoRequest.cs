using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Records.Request
{
    public class MetaDataTypologyBulksDtoRequest
    {
        public int? DocumentId { get; set; }
        public int MetaFieldId { get; set; }
        public int? MetaDataId { get; set; }
        public string? DataText { get; set; }
        public string? ColorData { get; set; }
    }
}
