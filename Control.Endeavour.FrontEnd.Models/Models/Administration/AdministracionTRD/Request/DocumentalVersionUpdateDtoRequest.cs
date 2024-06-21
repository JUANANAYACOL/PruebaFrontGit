using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request
{
    public class DocumentalVersionUpdateDtoRequest
    {
        public int DocumentalVersionId { get; set; }
        public int CompanyId { get; set; }
        public string VersionType { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null;
        public string Description { get; set; } = null;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? ActiveState { get; set; }
    }
}
