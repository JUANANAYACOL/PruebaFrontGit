using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request
{
    public class CcdDtoRequest
    {
        public int DocumentalVersionId { get; set; }
        public int? AdministrativeUnitId { get; set; }
        public int? ProductionOfficeId { get; set; }
    }
}
