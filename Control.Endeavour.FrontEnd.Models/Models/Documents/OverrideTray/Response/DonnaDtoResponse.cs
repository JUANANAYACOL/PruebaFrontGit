using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Response
{
    public class DonnaDtoResponse
    {
       
        public string NombreAnulado { get; set; }
        public string NombreDesanulado { get; set; }
        public string NombreRechazado { get; set; }
        public string NombrePendiente { get; set; }
        public int Anulado { get; set; }
        public int Desanulado { get; set; }
        public int Rechazado { get; set; }
        public int Pendiente { get; set; }
    }
}
