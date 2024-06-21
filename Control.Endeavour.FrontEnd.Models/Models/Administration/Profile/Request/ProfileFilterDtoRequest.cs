using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Profile.Request
{
    public class ProfileFilterDtoRequest : PaginationRequest
    {
        public string ProfileCode { get; set; }

        public string Profile1 { get; set; }
    }
}