using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Request
{
    public class OverrideTrayManagerEditDtoRequest
    {
        public int CancelationManagerId { get; set; }
        public int UserId { get; set; }
        public string TypeCode { get; set; }
        public bool ActiveState { get; set; }
        public string User { get; set; }
    }
}