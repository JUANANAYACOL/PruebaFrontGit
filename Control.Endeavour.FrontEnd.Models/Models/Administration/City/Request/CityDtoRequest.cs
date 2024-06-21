using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.City.Request
{
    public class CityDtoRequest : PaginationRequest
    {
        public string Name { get; set; }
        public string CodeNum { get; set; }
        public int? CityId { get; set; }
        public string CodeTxt { get; set; }
        public int StateId { get; set; }
        public string User { get; set; }
    }
}
