using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response
{
    public class ProdOfficeManagmentDtoResponse
    {
        public int ProductionOfficeId { get; set; }

        public int AdministrativeUnitId { get; set; }

        public int? PasswordExpirationTime { get; set; }

        public string Code { get; set; } = null!;

        public string? Name { get; set; }

        public string? CreateUser { get; set; }

        public string? UpdateUser { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
