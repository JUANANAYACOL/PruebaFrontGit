using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response
{
    public class AdministrativeActDtoResponse
    {
        public int AdministrativeActId { get; set; }
        public int DocumentalVersionId { get; set; }
        public string ActName { get; set; }
        public int FileId { get; set; }
        public byte[]? DataFile { get; set; }
        public DateTime? AdminActDate { get; set; }
    }
}
