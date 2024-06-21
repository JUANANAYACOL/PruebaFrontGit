using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Records.Response
{
    public class IndexDocumentaryTypologyDtoResponse
    {
        public int DocumentId { get; set; }
        public int ControlId { get; set; }
        public string? Description { get; set; }
        public string? Color { get; set; }
        public int StartFolio { get; set; }
        public int EndFolio { get; set; }
        public int NumberFolio { get; set; }
    }
}
