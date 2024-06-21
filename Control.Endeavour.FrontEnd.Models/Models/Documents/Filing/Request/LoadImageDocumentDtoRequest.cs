using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Filing.Request
{
    public class LoadImageDocumentDtoRequest
    {
        public decimal? ControlId { get; set; }
        public byte[]? ArchivoBytes { get; set; }
        public string? Ext { get; set; }
        public decimal? CompanyId { get; set; }
    }
}
