using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response
{
    public class DocumentalVersionDtoResponse
    {
        public int DocumentalVersionId { get; set; }
        public int CompanyId { get; set; }
        public string VersionType { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int FileOrganizationId { get; set; }
        public List<AdministrativeActDVDtoResponse> AdministrativeActs { get; set; }
        public bool? ActiveState { get; set; }

        public string CreateUser { get; set; }

        public DateTime? CreateDate { get; set; }

        public string UpdateUser { get; set; }

        public DateTime? UpdateDate { get; set; }
    }

    
}
