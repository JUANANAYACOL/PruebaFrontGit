

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request
{
    public class RetentionSSDtoRequest
    {
        
        public int TimeFileManagement { get; set; }

        public int TimeFileCentral { get; set; }

        public bool TotalConservation { get; set; }

        public bool Elimination { get; set; }

        public bool TechEnvironment { get; set; }

        public bool Selection { get; set; }

        public string ProcedureRet { get; set; } = null!;
    }
}
