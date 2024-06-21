using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request
{
    public class DocumentaryTypologiesBehaviorsUpdateDtoRequest
    {
        public int DocumentaryTypologyBehaviorId { get; set; }
        public int? DocumentaryTypologyId { get; set; }
        public string ClassCode { get; set; } = null!;
        public string CorrespondenceType { get; set; } = null!;
        public string User { get; set; }
    }
}
