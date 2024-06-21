using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request
{
    public class DocumentaryTypologiesBehaviorsFilterDtoRequest : PaginationRequest
    {
        public int DocumentaryTypologyBehaviorId { get; set; }
        public int DocumentaryTypologyId { get; set; }
        public string ClassCode { get; set; } = null!;
        public string CorrespondenceType { get; set; } = null!;
    }
}
