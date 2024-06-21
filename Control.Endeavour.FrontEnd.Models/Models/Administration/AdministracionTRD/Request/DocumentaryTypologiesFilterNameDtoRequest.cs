using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request
{
    public class DocumentaryTypologiesFilterNameDtoRequest : PaginationRequest
    {
        public int DocumentalVersionId { get; set; }
        public string DocumentalVersionName { get; set; }
        public int AdministrativeUnitId { get; set; }
        public string AdministrativeUnitName { get; set; }
        public int ProductionOfficeId { get; set; }
        public string ProductionOfficeName { get; set; }
        public int SeriesId { get; set; }
        public string SeriesName { get; set; }
        public int? SubseriesId { get; set; }
        public string SubseriesName { get; set; }
        public int DocumentaryTypologyBagId { get; set; }
        public string DocumentaryTypologyBagName { get; set; }
        public string? SecurityLevel { get; set; }
    }
}
