using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Filing.Response
{
    public class BulkFilingsDtoResponse
    {
        public bool Processed { get; set; }
        public string? Message { get; set; }
        public List<FilingDtoResponse> Filing { get; set; } = new();
    }
}