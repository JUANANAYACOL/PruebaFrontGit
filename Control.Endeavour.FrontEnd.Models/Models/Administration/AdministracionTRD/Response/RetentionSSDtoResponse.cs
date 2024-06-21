using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response
{
    public class RetentionSSDtoResponse
    {
        public int RetentionId { get; set; }

        public int TimeFileManagement { get; set; }

        public int TimeFileCentral { get; set; }

        public bool TotalConservation { get; set; }

        public bool Elimination { get; set; }

        public bool TechEnvironment { get; set; }

        public bool Selection { get; set; }

        public string ProcedureRet { get; set; } = null!;
    }
}
