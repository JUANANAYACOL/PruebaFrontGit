using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Request
{
    public class CancelationRequestQueryFilter : PaginationRequest
    {
        public int UserId { get; set; }
        public int controlId { get; set; }
        public bool UserManagerId { get; set; }
        public bool UserRequestId { get; set; }
        public string TypeRequestCode { get; set; }
        public int CancelationReasonId { get; set; }
        public string CancelationState { get; set; }
        public string TypeCode { get; set; }
    }
}