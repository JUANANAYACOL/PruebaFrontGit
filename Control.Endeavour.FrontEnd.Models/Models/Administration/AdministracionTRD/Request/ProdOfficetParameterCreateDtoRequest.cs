using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request
{
    public class ProdOfficetParameterCreateDtoRequest
    {
        public int ProductionOfficeId { get; set; }
        public int? PasswordExpirationTime { get; set; }
        public bool IsCreate { get; set; } = new();
    }
}