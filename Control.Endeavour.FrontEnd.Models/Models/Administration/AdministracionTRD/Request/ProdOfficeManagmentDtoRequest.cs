using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request
{
    public class ProdOfficeManagmentDtoRequest: PaginationRequest
    {
        public string? Name { get; set; }

        public string Code { get; set; } = null!;
    }
}
