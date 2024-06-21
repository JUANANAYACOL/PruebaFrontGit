using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request
{
    public class DocumentaryTypologiesFilterDtoRequest : PaginationRequest
    {
        public int? DocumentaryTypologyBagId { get; set; }
        public int? SeriesId { get; set; }
        public int? SubSeriesId { get; set; }
        public string? SecurityLevel { get; set; }
    }
}
