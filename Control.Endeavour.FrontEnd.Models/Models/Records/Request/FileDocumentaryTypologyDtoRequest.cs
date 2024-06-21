using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Records.Request
{
    public class FileDocumentaryTypologyDtoRequest
    {
        public byte[] DataFile { get; set; }
        public DateTime DocDate { get; set; } = DateTime.Now;
        public int VolumeId { get; set; }
        public int DocumentaryTypologyId { get; set; }
    }
}
