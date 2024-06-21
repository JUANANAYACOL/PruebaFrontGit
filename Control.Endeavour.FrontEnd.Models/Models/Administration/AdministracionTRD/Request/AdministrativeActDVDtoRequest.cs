using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request
{
    public class AdministrativeActDVDtoRequest
    {
        public string ActName { get; set; }
        public DateTime? AdminActDate { get; set; } = null;
        public string FileExt { get; set; }
        public string FileName { get; set; }
        public byte[] DataFile { get; set; }
    }
}
