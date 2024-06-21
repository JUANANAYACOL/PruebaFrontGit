using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request
{
    public class SeriesDtoRequest
    {
        public int ProductionOfficeId { get; set; }
        public string? Name { get; set; }
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public bool ActiveState { get; set; }
        public string? Class { get; set; }
        public string? SecurityLevel { get; set; }
        public RetentionSSDtoRequest? Retention { get; set; } = new RetentionSSDtoRequest();
    }
}
