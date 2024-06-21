using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Request
{
    public class OverrideTrayRequestUpdateDtoRequest
    {
        public int CancelationRequestId { get; set; }
        public string TypeRequestCode { get; set; }
        public int UserRequestId { get; set; }
        public string RejectionComment { get; set; } = string.Empty;
        public string CancelationState { get; set; }
    }
}
