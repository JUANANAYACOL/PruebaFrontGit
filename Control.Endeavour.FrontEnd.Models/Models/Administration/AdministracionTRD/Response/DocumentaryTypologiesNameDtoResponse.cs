using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response
{
    public class DocumentaryTypologiesNameDtoResponse
    {
        public int DocumentalVersionId { get; set; }
        public string DocumentalVersionCode { get; set; }
        public string DocumentalVersionName { get; set; }
        public int AdministrativeUnitId { get; set; }
        public string AdministrativeUnitCode { get; set; }
        public string AdministrativeUnitName { get; set; }
        public int ProductionOfficeId { get; set; }
        public string ProductionOfficeCode { get; set; }
        public string ProductionOfficeName { get; set; }
        public int SeriesId { get; set; }
        public string SeriesCode { get; set; }
        public string SeriesName { get; set; }
        public int? SubseriesId { get; set; }
        public string SubseriesCode { get; set; }
        public string SubseriesName { get; set; }
        public int DocumentaryTypologiesId { get; set; }
        public int DocumentaryTypologyBagId { get; set; }
        public bool ActiveState { get; set; }
        public string DocumentaryTypologyBagName { get; set; }
        public string? SecurityLevel { get; set; }
        public string CreateUser { get; set; } = null!;
        public string UpdateUser { get; set; } = null!;
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
