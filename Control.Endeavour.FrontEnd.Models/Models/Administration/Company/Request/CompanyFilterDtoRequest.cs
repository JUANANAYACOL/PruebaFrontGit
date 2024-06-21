using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Request
{
    public class CompanyFilterDtoRequest : PaginationRequest
    {
        public string IdentificationType { get; set; }
        public string Identification { get; set; }
        public string BusinessName { get; set; }
    }
}
