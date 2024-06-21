using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response
{
    public class AdministrativeUnitsDtoResponse
    {
        public int AdministrativeUnitId { get; set; }

        public int DocumentalVersionId { get; set; }

        public string DocumentalVersionName { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool ActiveState { get; set; }

        public string CreateUser { get; set; }

        public DateTime? CreateDate { get; set; }

        public string UpdateUser { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}