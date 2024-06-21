using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Records.Request
{
    public class IndexDocumentaryTypologyDtoRequest
    {
        public int StartFolio { get; set; }
        public int EndFolio { get; set; }
        public string? Observation { get; set; }
        public string? Color { get; set; }
        public string? Origin { get; set; }
        public string? Options { get; set; }
        public string? OptionValue { get; set; }
        public int VolumeId { get; set; }
        public int DocumentaryTypologyId { get; set; }
    }
}
