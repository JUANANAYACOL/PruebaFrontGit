using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request
{
    public class SeriesFilterDtoRequest : PaginationRequest
    {
        public int? ProductionOfficeId { get; set; }
        public int? AdministrativeUnitId { get; set; }
        public int? DocumentalVersionId { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
    }
}
