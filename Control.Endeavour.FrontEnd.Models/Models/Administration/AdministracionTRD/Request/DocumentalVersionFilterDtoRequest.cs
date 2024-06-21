using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request
{
    public class DocumentalVersionFilterDtoRequest : PaginationRequest
    {
        public int CompanyId { get; set; }
        public string VersionType { get; set; }
        public string Code { get; set; }
        public string Name { get; set; } = null;
        public bool? CurrentVersion { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
