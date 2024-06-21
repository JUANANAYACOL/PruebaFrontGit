using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Request
{
    public class OverrideTrayManagerDtoRequest : PaginationRequest
    {
        public int UserId { get; set; }
        public string TypeCode { get; set; }
        public string User { get; set; }
        public bool ActiveState { get; set; }
    }
}