using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Records.Response
{
    public class ViewIndexedPDFDtoResponse
    {
        public int DocumentId { get; set; }
        public byte[]? DataFile { get; set; }
    }
}
