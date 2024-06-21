using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response
{
    public class GetImageDocumentDtoResponse
    {
        public int ControlId { get; set; }
        public int FileId { get; set; }
        public byte[]? DataFile { get; set; }
    }
}
