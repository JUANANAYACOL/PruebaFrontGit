using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Filing.Request
{
    public class BulkFilingsDtoRequest
    {
        public string? ClassCode { get; set; }
        public string? FileExt { get; set; }
        public byte[]? Archive { get; set; }
        public int UserId { get; set; }
    }
}
